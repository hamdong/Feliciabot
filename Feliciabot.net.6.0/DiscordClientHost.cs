using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Feliciabot.net._6._0
{
    internal sealed class DiscordClientHost : IHostedService
    {
        private readonly string clientTokenPath =
            Environment.CurrentDirectory + @"\ignore\token.txt";

        private readonly DiscordSocketClient _client;
        private readonly ILogger<DiscordClientHost> _logger;
        private readonly CommandService _commands;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGreetingService _greetingService;

        public DiscordClientHost(
            DiscordSocketClient discordSocketClient,
            ILogger<DiscordClientHost> logger,
            CommandService commandService,
            InteractionService interactionService,
            IServiceProvider serviceProvider,
            IGreetingService greetingService
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

            try
            {
                _client.Log += LogHandler;
                _commands.Log += LogHandler;

                if (!File.Exists(clientTokenPath))
                {
                    Console.WriteLine("Can't find token. Aborting.");
                    Console.ReadLine();
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

            await _client
                .LoginAsync(TokenType.Bot, File.ReadAllText(clientTokenPath))
                .ConfigureAwait(false);

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

        private Task InteractionCreated(SocketInteraction interaction)
        {
            var interactionContext = new SocketInteractionContext(_client, interaction);
            return _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);
        }

        private async Task ClientReady()
        {
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
            if (message.HasCharPrefix('!', ref argPos))
            {
                // Execute the command with command context
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

        public static Task LogHandler(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            string msg =
                $"{DateTime.Now, -19} [{message.Severity, 8}] {message.Source}: {message.Message} {message.Exception}";
            Console.WriteLine(msg);
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
