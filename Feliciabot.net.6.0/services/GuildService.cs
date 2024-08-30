using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class GuildService(DiscordSocketClient client, IGuildFactory guildFactory) : IGuildService
    {
        public Channel? GetChannelByGuildById(ulong guildId, ulong channelId)
        {
            return guildFactory.FromSocketGuild(client.GetGuild(guildId)).Channels.ToList().Find(c => c.Id == channelId);
        }

        public async Task AddRoleToUserByIdAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var user = guildFactory.FromSocketGuild(client.GetGuild(guildId)).Users.ToList().Find(u => u.Id == userId);
            if (user == null) return;
            await user.AddRoleByIdAsync(roleId);
        }

        public ulong GetRoleIdByName(ulong guildId, string name)
        {
            var role = guildFactory.FromSocketGuild(client.GetGuild(guildId)).Roles.ToList().Find(r => r.Name == name);
            return role?.Id ?? default; // 0 if not found
        }
    }
}
