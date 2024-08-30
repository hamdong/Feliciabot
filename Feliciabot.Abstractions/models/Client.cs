using Discord;
using Discord.WebSocket;

namespace Feliciabot.Abstractions.models
{
    public class Client
    {
        public string Username { get; set; }
        public UserStatus Status { get; set; }
        public IReadOnlyCollection<IActivity> Activities { get; set; }

        public IReadOnlyCollection<SocketGuild> Guilds { get; set; }

        public Client(string username, UserStatus status, IActivity activity)
        {
            Username = username;
            Status = status;
            Activities = [activity];
            Guilds = [];
        }

        public Client(DiscordSocketClient client)
        {
            Username = client.CurrentUser.Username;
            Status = client.CurrentUser.Status;
            Activities = client.CurrentUser.Activities;
            Guilds = client.Guilds;
        }

        public static Client FromDiscordSocketClient(DiscordSocketClient client)
        {
            return new Client(client);
        }
    }
}
