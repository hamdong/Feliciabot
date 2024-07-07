using Discord.WebSocket;

namespace Feliciabot.net._6._0.services
{
    public class GuildService(DiscordSocketClient client)
    {
        public SocketGuild GetGuildById(ulong guildId)
        {
            return client.GetGuild(guildId);
        }

        public SocketUser GetUserFromGuildById(ulong guildId, ulong userId)
        {
            return client.GetGuild(guildId).GetUser(userId);
        }

        public virtual async Task AddRoleToUserByIdAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var user = client.GetGuild(guildId).GetUser(userId);
            if (user == null)
            {
                return;
            }
            await user.AddRoleAsync(roleId);
        }

        public IReadOnlyCollection<string> GetAllRoleNames(ulong guildId)
        {
            return client.GetGuild(guildId).Roles.Select(role => role.Name).ToList().AsReadOnly();
        }

        public virtual ulong GetRoleIdByName(ulong guildId, string name)
        {
            var role = client.GetGuild(guildId).Roles.FirstOrDefault(r => r.Name == name);
            return role?.Id ?? default; // 0 if not found
        }

        public IReadOnlyCollection<SocketRole> GetRoles(ulong guildId)
        {
            return client.GetGuild(guildId).Roles;
        }

        public SocketGuildUser GetUser(ulong guildId, ulong id)
        {
            return client.GetGuild(guildId).GetUser(id);
        }
    }
}
