using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;
using WaifuSharp;

namespace Feliciabot.net._6._0.services
{
    public class WaifuSharpService(WaifuClient waifuClient) : InteractionModuleBase<SocketInteractionContext>, IWaifuSharpService
    {
        public async Task SendWaifuSharpResponseAsync(IInteractionContext context, Endpoints.Sfw action, string actionOnUser)
        {
            string message = $"{context.User.GlobalName} {actionOnUser}";
            var builder = new EmbedBuilder();
            string imgURL = waifuClient.GetSfwImage(action);
            builder.WithImageUrl(imgURL);

            await context.Interaction.RespondAsync(message, embed: builder.Build()).ConfigureAwait(false);
        }
    }
}
