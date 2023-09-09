using BooruSharp.Booru;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.services;
using Fergun.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Victoria.Node;
using WaifuSharp;
using YoutubeSearchApi.Net.Services;

namespace Feliciabot.net._6._0
{
    internal class Main
    {
        private DiscordSocketClient _client = new();
        private CommandService _commands = new();
        private readonly string clientTokenPath = Environment.CurrentDirectory + @"\ignore\token.txt";

        /// <summary>
        /// Asynchronous call to log in and setup command services
        /// </summary>
        public async Task MainAsync()
        {
            try
            {
                _client = new DiscordSocketClient(GenerateNewConfig());
                _commands = new CommandService();

                // Subscribe the logging handler to both the client and the CommandService.
                _client.Log += LogHandler;
                _commands.Log += LogHandler;
                LogHelper.ClearPreviousLogs();

                if (!File.Exists(clientTokenPath))
                {
                    Console.WriteLine("Can't find token. Aborting.");
                    Console.ReadLine();
                    return;
                }

                // Login and connect
                string token = File.ReadAllText(clientTokenPath);
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
                await _client.SetGameAsync("!icanhelp");
            }
            catch (OperationCanceledException ex)
            {
                // Check ex.CancellationToken.IsCancellationRequested here.
                // If false, it's pretty safe to assume it was a timeout.
                if (!ex.CancellationToken.IsCancellationRequested) LogHelper.Log(ex.Message);
            }

            // Initialize commands
            await BuildServiceProvider().GetRequiredService<CommandHandler>().InitializeAsync();

            // Block this task until the program is closed
            await Task.Delay(-1);
        }

        /// <summary>
        /// Service Registration
        /// </summary>
        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<WaifuClient>()
            .AddSingleton<HttpClient>()
            .AddSingleton<InteractiveService>()
            .AddSingleton<Gelbooru>()
            .AddSingleton<YoutubeSearchClient>()
            .AddLogging()
            .AddSingleton<NodeConfiguration>()
            .AddSingleton<LavaNode>()
            .AddSingleton<AudioService>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();

        /// <summary>
        /// Log Handler
        /// </summary>
        /// <param name="message">Message received to log</param>
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
            string msg = $"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}";
            LogHelper.Log(msg);
            Console.WriteLine(msg);
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new config file to be used with the client
        /// </summary>
        private static DiscordSocketConfig GenerateNewConfig()
        {
            var config = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
            };
            config.GatewayIntents &= ~GatewayIntents.GuildScheduledEvents;
            config.GatewayIntents &= ~GatewayIntents.GuildInvites;
            return config;
        }
    }
}
