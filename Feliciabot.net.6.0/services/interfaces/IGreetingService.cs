using Discord.WebSocket;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IGreetingService
    {
        public Task ReplyToNonCommand(SocketUserMessage message);
        public Task HandleOnUserJoined(SocketGuildUser user);
        public Task HandleOnUserLeft(SocketGuild guild, SocketUser user);
    }
}
