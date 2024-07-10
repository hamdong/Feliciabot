using Discord.Commands;

namespace Feliciabot.net._6._0.services
{
    public class MessagingService(ClientService clientService) : ModuleBase<SocketCommandContext>
    {
        public virtual async Task SendMessageToContext(ICommandContext context, string message)
        {
            var channel = clientService.GetChannelByGuildById(context.Guild.Id, context.Channel.Id);
            if (channel == null) return;
            await channel.SendMessageToChannelAsync(message);
        }
    }
}
