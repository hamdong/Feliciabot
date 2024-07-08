using Discord.WebSocket;
using Feliciabot.net._6._0.models;

namespace Feliciabot.net._6._0.services
{
    public class ClientService(DiscordSocketClient client)
    {
        public virtual Guild GetGuildById(ulong id)
        {
            return Guild.FromSocketGuild(client.GetGuild(id));
        }
        public virtual User? GetUserByGuildById(ulong guildId, ulong userId)
        {
            var user = client.GetGuild(guildId).GetUser(userId);
            if (user == null) return null;
            return User.FromSocketGuildUser(user);
        }
    }
}
