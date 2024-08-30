using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.Abstractions.models;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IInteractingService
    {
        public Task SendResponseAsync(SocketInteractionContext<SocketInteraction> context, string message);
        public Task SendRollResponseAsync(SocketInteractionContext<SocketInteraction> context, int roll);
        public Task SendFlipResponseAsync(SocketInteractionContext<SocketInteraction> context, string flip);
        public Task SendResponseToUserAsync(SocketInteractionContext<SocketInteraction> context, Embed message);
        public IReadOnlyList<SlashCommand> GetSlashCommands();
    }
}
