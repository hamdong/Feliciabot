using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class InteractingService : IInteractingService
    {
        public async Task SendResponseAsync(SocketInteractionContext<SocketInteraction> interaction, string message)
        {
            await interaction.Interaction.RespondAsync(message).ConfigureAwait(false);
        }

        public async Task SendRollResponseAsync(SocketInteractionContext<SocketInteraction> interaction, int roll)
        {
            await interaction.Interaction.RespondAsync($"{interaction.User.GlobalName} rolled *{roll}*").ConfigureAwait(false);
        }

        public async Task SendFlipResponseAsync(SocketInteractionContext<SocketInteraction> interaction, string flip)
        {
            await interaction.Interaction.RespondAsync($"{interaction.User.GlobalName} got *{flip}*").ConfigureAwait(false);
        }

        public async Task SendResponseToUserAsync(SocketInteractionContext<SocketInteraction> interaction, Embed message)
        {
            await interaction.User.SendMessageAsync(embed: message).ContinueWith(r => interaction.Interaction.RespondAsync()).ConfigureAwait(false);
        }
    }
}
