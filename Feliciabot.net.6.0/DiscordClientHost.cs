using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
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

        public DiscordClientHost(
            DiscordSocketClient discordSocketClient,
            ILogger<DiscordClientHost> logger,
            CommandService commandService,
            InteractionService interactionService,
            IServiceProvider serviceProvider,
            IGreetingService greetingService,
            IOptions<BotSettings> botSettings
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

            try
            {
                _client.Log += LogAsync;
                _commands.Log += LogAsync;

                var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Can't find token. Aborting.");
                    return;
                }
            }
            catch (OperationCanceledException e)
            {
                // Check ex.CancellationToken.IsCancellationRequested here.
                // If false, it's pretty safe to assume it was a timeout.
                if (!e.CancellationToken.IsCancellationRequested)
                    _logger.LogError("{Message}", e.Message);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _client.UserJoined += OnUserJoined;
            _client.UserLeft += OnUserLeft;
            _client.InteractionCreated += InteractionCreated;
            _client.Ready += ClientReady;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _serviceProvider
            );

            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            await _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);

            await _client.StartAsync().ConfigureAwait(false);
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

        private Task<Discord.Interactions.IResult> InteractionCreated(SocketInteraction interaction)
        {
            var interactionContext = new SocketInteractionContext(_client, interaction);
            return _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);
        }

        private async Task ClientReady()
        {
            if (_interactionService.Modules.Count > 0)
                return;

            await _client.SetGameAsync("!icanhelp");

            await _interactionService
                .AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider)
                .ConfigureAwait(false);

            await _interactionService.RegisterCommandsGloballyAsync().ConfigureAwait(false);
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
                await _commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _serviceProvider
                );
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
