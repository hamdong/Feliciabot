using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WaifuSharp;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IWaifuSharpService
    {
        public Task SendWaifuSharpResponseAsync(IInteractionContext context, Endpoints.Sfw action, string actionOnUser);
    }
}
