using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;

namespace Feliciabot.Abstractions.factories
{
    public class ClientFactory : IClientFactory
    {
        public Client FromDiscordSocketClient(DiscordSocketClient client)
        {
            return Client.FromDiscordSocketClient(client);
        }
    }
}
