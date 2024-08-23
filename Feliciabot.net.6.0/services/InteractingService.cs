using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class InteractingService : IInteractingService
    {
        private readonly InteractionService _interactionService;
        public InteractingService(InteractionService interactionService)
        {
            _interactionService = interactionService;    
        }

        public async Task SendResponseAsync(SocketInteractionContext<SocketInteraction> context, string message)
        {
            await context.Interaction.RespondAsync(message).ConfigureAwait(false);
        }

        public async Task SendRollResponseAsync(SocketInteractionContext<SocketInteraction> context, int roll)
        {
            await context.Interaction.RespondAsync($"{context.User.GlobalName} rolled *{roll}*").ConfigureAwait(false);
        }

        public async Task SendFlipResponseAsync(SocketInteractionContext<SocketInteraction> context, string flip)
        {
            await context.Interaction.RespondAsync($"{context.User.GlobalName} got *{flip}*").ConfigureAwait(false);
        }

        public async Task SendResponseToUserAsync(SocketInteractionContext<SocketInteraction> context, Embed message)
        {
            await context.Interaction.DeferAsync();
            await context.User.SendMessageAsync(embed: message).ConfigureAwait(false);
            await context.Interaction.FollowupAsync("DM sent!");
        }

        public IReadOnlyList<SlashCommand> GetSlashCommands()
        {
            return Interaction.FromSlashCommandList(_interactionService.SlashCommands).SlashCommands;
        }
    }
}
