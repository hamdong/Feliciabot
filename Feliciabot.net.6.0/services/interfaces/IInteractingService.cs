using Discord.Interactions;
using Discord.WebSocket;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IInteractingService
    {
        Task SendRespondAsync(SocketInteractionContext<SocketInteraction> interaction, string message);
    }
}
