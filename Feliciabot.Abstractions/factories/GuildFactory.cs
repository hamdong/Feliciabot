using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;

namespace Feliciabot.Abstractions.factories
{
    public class GuildFactory : IGuildFactory
    {
        public Guild FromSocketGuild(SocketGuild socketGuild)
        {
            return Guild.FromSocketGuild(socketGuild);
        }
    }
}
