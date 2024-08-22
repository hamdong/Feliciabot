using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IPaginatorService
    {
        public StaticPaginator BuildPaginator(SocketInteractionContext<SocketInteraction> context, string moduleName);
        public Task SendPaginatorAsync(SocketInteractionContext<SocketInteraction> context, StaticPaginator paginator);
    }
}
