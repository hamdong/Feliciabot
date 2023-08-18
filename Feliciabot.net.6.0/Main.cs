using BooruSharp.Booru;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.services;
using Fergun.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;
using Victoria.Node;
using WaifuSharp;

namespace Feliciabot.net._6._0
{
    internal class Main
    {
        private DiscordSocketClient _client = new();
        private CommandService _commands = new();
        private LavaNode _avaNode = new(new DiscordSocketClient(), new NodeConfiguration(), null);
        private readonly string clientTokenPath = Environment.CurrentDirectory + @"\ignore\token.txt";

        /// <summary>
        /// Asynchronous call to log in and setup command services
        /// </summary>
        /// <returns>Nothing, run until manually closed</returns>
        public async Task MainAsync()
        {
            //Server login
            try
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
                });
                _commands = new CommandService();
                var loggerFactory = (ILoggerFactory)new LoggerFactory();
                _avaNode = new LavaNode(_client, new NodeConfiguration(), loggerFactory.CreateLogger<LavaNode>());

                // Subscribe the logging handler to both the client and the CommandService.
                _client.Log += LogHandler;
                _commands.Log += LogHandler;

                //Clear previous logs
                LogHelper.ClearPreviousLogs();

                if (File.Exists(clientTokenPath))
                {
                    string token = File.ReadAllText(clientTokenPath);

                    // Login and connect.
                    await _client.LoginAsync(TokenType.Bot, token);
                    await _client.StartAsync();
                    await _client.SetGameAsync("!icanhelp");
                }
                else
                {
                    Console.WriteLine("Can't find token. Aborting.");
                    Console.ReadLine();
                }
            }
            catch (OperationCanceledException ex)
            {
                // Check ex.CancellationToken.IsCancellationRequested here.
                // If false, it's pretty safe to assume it was a timeout.
                if (!ex.CancellationToken.IsCancellationRequested)
                {
                    LogHelper.Log(ex.Message);
                }
            }

            // Initialize commands
            await BuildServiceProvider().GetRequiredService<CommandHandler>().InitializeAsync();
            BuildServiceProvider().GetRequiredService<AudioService>().Initialize();

            // Block this task until the program is closed
            await Task.Delay(-1);
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<WaifuClient>()
            .AddSingleton<HttpClient>()
            .AddSingleton<InteractiveService>()
            .AddSingleton<Gelbooru>()
            .AddLogging()
            .AddLavaNode(x => x.SelfDeaf = true)
            .AddSingleton(_avaNode)
            .AddSingleton<AudioService>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();

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
    }
}
