using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;

namespace Feliciabot.Abstractions.models
{
    public class User : IUser
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

        public static User? FromSocketGuildUser(SocketGuildUser socketGuildUser)
        {
            return socketGuildUser == null ? null : new User(socketGuildUser);
        }

        public static User[] FromSocketGuildUsers(IReadOnlyCollection<SocketGuildUser> socketGuildUsers)
        {
            return socketGuildUsers.Select(socketGuildUser => new User(socketGuildUser)).ToArray();
        }
    }
}
