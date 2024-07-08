using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class ClientService(DiscordSocketClient client, IGuildFactory guildFactory) : IClientService
    {
        public virtual Guild GetGuildById(ulong id)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(id));
        }
        public virtual User? GetUserByGuildById(ulong guildId, ulong userId)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(guildId)).Users.ToList().Find(u => u.Id == userId);
        }
    }
}
