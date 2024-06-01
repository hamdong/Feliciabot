using Discord;
using Discord.WebSocket;

namespace Feliciabot.net._6._0
{
    public static class Config
    {
        public static DiscordSocketConfig GenerateNewConfig()
        {
            var config = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.GuildPresences,
                AlwaysDownloadUsers = true,
            };
            config.GatewayIntents &= ~GatewayIntents.GuildScheduledEvents;
            config.GatewayIntents &= ~GatewayIntents.GuildInvites;
            return config;
        }
    }
}
