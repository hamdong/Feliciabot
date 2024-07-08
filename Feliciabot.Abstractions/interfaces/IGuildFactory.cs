using Discord.WebSocket;
using Feliciabot.Abstractions.models;

namespace Feliciabot.Abstractions.interfaces
{
    public interface IGuildFactory
    {
        Guild FromSocketGuild(SocketGuild socketGuild);
    }
}
