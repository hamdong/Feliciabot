using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class ClientService(DiscordSocketClient client) : IClientService
    {
        public ulong GetClientId()
        {
            return client.CurrentUser.Id;
        }
    }
}
