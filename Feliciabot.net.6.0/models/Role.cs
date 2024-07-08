using Discord.WebSocket;

namespace Feliciabot.net._6._0.models
{
    public class Role
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong GuildId { get; set; }
        public string GuildName { get; set; }

        public Role()
        {
            Id = 0;
            Name = "role";
            GuildId = 0;
            GuildName = "guild";
        }

        public Role(ulong id, string name, ulong guildId, string guildName)
        {
            Id = id;
            Name = name;
            GuildId = guildId;
            GuildName = guildName;
        }

        public Role(SocketRole socketRole)
        {
            Id = socketRole.Id;
            Name = socketRole.Name;
            GuildId = socketRole.Guild.Id;
            GuildName = socketRole.Guild.Name;
        }

        public static Role FromSocketRole(SocketRole socketRole)
        {
            return new Role(socketRole);
        }

        public static Role[] FromSocketRoles(IReadOnlyCollection<SocketRole> socketRoles)
        {
            return socketRoles.Select(socketRole => new Role(socketRole)).ToArray();
        }
    }
}
