using System.Diagnostics.CodeAnalysis;
using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services
{
    [ExcludeFromCodeCoverage]
    public class FergunInteractiveService(InteractiveService _interactiveService)
        : IFergunInteractiveService
    {
        public async Task SendPaginatorAsync(
            StaticPaginator paginator,
            ISocketMessageChannel channel,
            TimeSpan activeTime
        )
        {
            await _interactiveService
                .SendPaginatorAsync(paginator, channel, activeTime)
                .ConfigureAwait(false);
        }
    }
}
