using Discord.WebSocket;

namespace Feliciabot.net._6._0.models
{
    public class User
    {
        private SocketGuildUser? SocketGuildUser { get; set; }
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public ulong DiscriminatorValue => Convert.ToUInt64(Discriminator[^4..]);

        public User()
        {
            Id = 0;
            Username = "user";
            Discriminator = "0000";
        }

        public User(ulong id, string username, string discriminator)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
        }

        public User(SocketGuildUser socketGuildUser)
        {
            Id = socketGuildUser.Id;
            Username = socketGuildUser.Username;
            Discriminator = socketGuildUser.Discriminator;
            SocketGuildUser = socketGuildUser;
        }

        public virtual async Task AddRoleByIdAsync(ulong roleId)
        {
            if (SocketGuildUser != null) await SocketGuildUser.AddRoleAsync(roleId);
        }

        public static User FromSocketGuildUser(SocketGuildUser socketGuildUser)
        {
            return new User(socketGuildUser);
        }
    }
}
