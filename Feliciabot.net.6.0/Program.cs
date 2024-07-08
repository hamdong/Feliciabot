// See https://aka.ms/new-console-template for more information
using BooruSharp.Booru;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.Abstractions.factories;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.net._6._0;
using Feliciabot.net._6._0.services;
using Fergun.Interactive;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaifuSharp;
using YoutubeSearchApi.Net.Services;

try
{
    var builder = new HostApplicationBuilder(args);

    // Config
    var config = Config.GenerateNewConfig();
    var discordSocketClient = new DiscordSocketClient(config);

    // Discord
    builder.Services.AddSingleton(discordSocketClient);
    builder.Services.AddSingleton<CommandService>();
    builder.Services.AddSingleton(new InteractionService(discordSocketClient.Rest, new InteractionServiceConfig { AutoServiceScopes = true }));
    builder.Services.AddHostedService<DiscordClientHost>();

    // Lavalink
    builder.Services.AddLavalink();
    builder.Services.AddInactivityTracking();
    builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));
    builder.Services.ConfigureInactivityTracking(x => { })
        .Configure<UsersInactivityTrackerOptions>(options =>
        {
            options.Threshold = 1;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.ExcludeBots = true;
        });

    // Abstractions
    builder.Services.AddScoped<IGuildFactory, GuildFactory>();

    // Services
    builder.Services.AddHostedService<BirthdayService>()
        .AddSingleton<ClientService>()
        .AddSingleton<GuildService>()
        .AddSingleton<GreetingService>()
        .AddSingleton<UserManagementService>()
        .AddSingleton<EmbedBuilderService>();

    // Misc.
    builder.Services.AddSingleton<WaifuClient>()
        .AddSingleton<HttpClient>()
        .AddSingleton<InteractiveService>()
        .AddSingleton<Gelbooru>()
        .AddSingleton<YoutubeSearchClient>();

    builder.Build().Run();
}
catch (FileNotFoundException e)
{
    Console.WriteLine(e.Message);
    Console.ReadLine();
}
