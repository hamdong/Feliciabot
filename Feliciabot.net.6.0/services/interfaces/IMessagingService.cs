using Discord.Commands;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IMessagingService
    {
        public Task SendMessageToContextAsync(ICommandContext context, string message);
        public Task SendMessageToChannelByIdAsync(ulong guildId, ulong channelId, string message);
    }
}
