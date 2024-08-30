using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class ClientService(DiscordSocketClient client, IClientFactory clientFactory) : IClientService
    {
        public Client GetClient()
        {
            return clientFactory.FromDiscordSocketClient(client);
        }
    }
}
