using Discord;
using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class ClientService(DiscordSocketClient client, IGuildFactory guildFactory, IClientFactory clientFactory) : IClientService
    {
        public Client GetClient()
        {
            return clientFactory.FromDiscordSocketClient(client);
        }

        public Guild GetGuildById(ulong id)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(id));
        }
        public User? GetUserByGuildById(ulong guildId, ulong userId)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(guildId)).Users.ToList().Find(u => u.Id == userId);
        }

        public Channel? GetChannelByGuildById(ulong guildId, ulong channelId)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(guildId)).Channels.ToList().Find(c => c.Id == channelId);
        }
    }
}
