using Discord.WebSocket;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IFergunInteractiveService
    {
        public Task SendPaginatorAsync(
            StaticPaginator paginator,
            ISocketMessageChannel channel,
            TimeSpan activeTime
        );
    }
}
