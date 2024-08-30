using Discord.WebSocket;
using Feliciabot.Abstractions.models;

namespace Feliciabot.Abstractions.interfaces
{
    public interface IClientFactory
    {
        Client FromDiscordSocketClient(DiscordSocketClient client);
    }
}
