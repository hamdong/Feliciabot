using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Feliciabot.net._6._0
{
    internal sealed class DiscordClientHost : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<DiscordClientHost> _logger;
        private readonly CommandService _commands;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGreetingService _greetingService;
        private readonly BotSettings _botSettings;
        private readonly MetricsService _metricsService;

        public DiscordClientHost(
            DiscordSocketClient discordSocketClient,
            ILogger<DiscordClientHost> logger,
            CommandService commandService,
            InteractionService interactionService,
            IServiceProvider serviceProvider,
            IGreetingService greetingService,
            IOptions<BotSettings> botSettings,
            MetricsService metricsService
        )
        {
            ArgumentNullException.ThrowIfNull(discordSocketClient);
            ArgumentNullException.ThrowIfNull(interactionService);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _client = discordSocketClient;
            _logger = logger;
            _commands = commandService;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
            _greetingService = greetingService;
            _botSettings = botSettings.Value;
            _metricsService = metricsService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _client.UserJoined += OnUserJoined;
            _client.UserLeft += OnUserLeft;
            _client.InteractionCreated += InteractionCreated;
            _client.Ready += ClientReady;

            _client.Log += LogAsync;
            _commands.Log += LogAsync;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _serviceProvider
            );

            try
            {
                var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Can't find token. Aborting.");
                    return;
                }

                await _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);

                await _client.StartAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                // Check ex.CancellationToken.IsCancellationRequested here.
                // If false, it's pretty safe to assume it was a timeout.
                if (!e.CancellationToken.IsCancellationRequested)
                    _logger.LogError("{Message}", e.Message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived -= OnMessageReceived;
            _client.UserJoined -= OnUserJoined;
            _client.UserLeft -= OnUserLeft;
            _client.InteractionCreated -= InteractionCreated;
            _client.Ready -= ClientReady;

            await _client.StopAsync().ConfigureAwait(false);
        }

        private async Task<Discord.Interactions.IResult> InteractionCreated(
            SocketInteraction interaction
        )
        {
            var interactionContext = new SocketInteractionContext(_client, interaction);

            string commandName = "unknown";

            if (interaction is SocketSlashCommand slashCommand)
            {
                commandName = slashCommand.Data.Name;
            }

            var result = await _interactionService.ExecuteCommandAsync(interactionContext, _serviceProvider);

            if (result.IsSuccess)
            {
                _metricsService.IncSlashCommand(commandName);
            }
            else
            {
                _metricsService.IncCommandError(commandName);
            }


            return result;
        }

        private async Task ClientReady()
        {
            if (_interactionService.Modules.Count == 0)
            {
                await _interactionService.AddModulesAsync(
                    Assembly.GetExecutingAssembly(),
                    _serviceProvider
                );
                await _interactionService.RegisterCommandsGloballyAsync();
            }

            await _client.SetGameAsync("!icanhelp");

            _metricsService.SetGuildCount(_client.Guilds.Count);
            _metricsService.SetUserCount(_client.Guilds.Sum(g => g.MemberCount));
        }

        private async Task OnMessageReceived(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message or bot
            if (
                messageParam is not IUserMessage message
                || message.Channel == null
                || message.Author.IsBot
            )
                return;

            int argPos = 0;

            if (message.HasCharPrefix(_botSettings.CommandPrefix, ref argPos))
            {
                var context = new SocketCommandContext(_client, (SocketUserMessage)message);
                var commandName = context.Message.Content.Split(' ')[0];

                var result = await _commands.ExecuteAsync(context, argPos, _serviceProvider);

                if (result.IsSuccess)
                {
                    _metricsService.IncCommand(commandName);
                }
                else
                {
                    _metricsService.IncCommandError(commandName);
                }


                return;
            }

            await _greetingService.ReplyToNonCommand(message);
        }

        public async Task OnUserJoined(SocketGuildUser user)
        {
            await _greetingService.HandleOnUserJoined(user);
        }

        public async Task OnUserLeft(SocketGuild guild, SocketUser user)
        {
            await _greetingService.HandleOnUserLeft(guild, user);
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine(
                    $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                        + $" failed to execute in {cmdException.Context.Channel}."
                );
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }
}
