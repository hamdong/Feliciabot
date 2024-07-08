using Discord.WebSocket;

namespace Feliciabot.net._6._0.models
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public virtual Role[] Roles { get; set; }

        public Guild()
        {
            Id = 0;
            Name = "guild";
            Roles = [];
        }

        public Guild(ulong id, string name)
        {
            Id = id;
            Name = name;
            Roles = [];
        }

        public Guild(SocketGuild socketGuild)
        {
            Id = socketGuild.Id;
            Name = socketGuild.Name;
            Roles = Role.FromSocketRoles(socketGuild.Roles);
        }

        public static Guild FromSocketGuild(SocketGuild socketGuild)
        {
            return new Guild(socketGuild);
        }
    }
}
