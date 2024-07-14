using Discord.Interactions;
using Discord.WebSocket;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IInteractingService
    {
        Task SendResponseAsync(SocketInteractionContext<SocketInteraction> interaction, string message);
        Task SendRollResponseAsync(SocketInteractionContext<SocketInteraction> interaction, int roll);
        Task SendFlipResponseAsync(SocketInteractionContext<SocketInteraction> interaction, string flip);
    }
}
