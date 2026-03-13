using Discord;

namespace Feliciabot.services.interfaces
{
    public interface IGreetingService
    {
        public Task ReplyToNonCommand(IUserMessage message);
        public Task HandleOnUserJoined(IGuildUser user);
        public Task HandleOnUserLeft(IGuild guild, IUser user);
    }
}
