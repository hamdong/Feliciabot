using Discord.WebSocket;
using Fergun.Interactive.Pagination;

namespace Feliciabot.services.interfaces
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
