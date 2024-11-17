using Discord;
using Discord.WebSocket;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IGreetingService
    {
        public Task ReplyToNonCommand(IUserMessage message);
        public Task HandleOnUserJoined(IGuildUser user);
        public Task HandleOnUserLeft(IGuild guild, IUser user);
    }
}
