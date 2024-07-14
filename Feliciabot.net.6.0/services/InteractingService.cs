using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class InteractingService : IInteractingService
    {
        public async Task SendRespondAsync(SocketInteractionContext<SocketInteraction> interaction, string message)
        {
            await interaction.Interaction.RespondAsync(message).ConfigureAwait(false);
        }
    }
}
