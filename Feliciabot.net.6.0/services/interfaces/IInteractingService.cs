using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IInteractingService
    {
        Task SendResponseAsync(SocketInteractionContext<SocketInteraction> context, string message);
        Task SendRollResponseAsync(SocketInteractionContext<SocketInteraction> context, int roll);
        Task SendFlipResponseAsync(SocketInteractionContext<SocketInteraction> context, string flip);
        Task SendResponseToUserAsync(SocketInteractionContext<SocketInteraction> context, Embed message);
    }
}
