// See https://aka.ms/new-console-template for more information
using BooruSharp.Booru;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WaifuSharp;
using YoutubeSearchApi.Net.Services;

var builder = new HostApplicationBuilder(args);

// Config
var config = Config.GenerateNewConfig();
var discordSocketClient = new DiscordSocketClient(config);

// Discord
builder.Services.AddSingleton(discordSocketClient);
builder.Services.AddSingleton<IDiscordClient>(discordSocketClient);
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton(
    new InteractionService(
        discordSocketClient.Rest,
        new InteractionServiceConfig { AutoServiceScopes = true }
    )
);
builder.Services.AddHostedService<DiscordClientHost>();

// Lavalink
builder.Services.AddLavalink();
builder.Services.AddInactivityTracking();
builder.Services.AddLogging();
builder
    .Services.ConfigureInactivityTracking(x => { })
    .Configure<UsersInactivityTrackerOptions>(options =>
    {
        options.Threshold = 1;
        options.Timeout = TimeSpan.FromSeconds(30);
        options.ExcludeBots = true;
    });

// Services
builder
    .Services.AddSingleton<IInteractiveHelperService, InteractiveHelperService>()
    .AddSingleton<IWaifuSharpService, WaifuSharpService>()
    .AddSingleton<IRandomizerService, RandomizerService>()
    .AddSingleton<IGreetingService, GreetingService>()
    .AddSingleton<IUserManagementService, UserManagementService>()
    .AddSingleton<IEmbedBuilderService, EmbedBuilderService>();

// Misc.
builder
    .Services.AddSingleton<WaifuClient>()
    .AddSingleton<InteractiveService>()
    .AddSingleton<Gelbooru>()
    .AddSingleton<YoutubeSearchClient>();

await builder.Build().RunAsync();
