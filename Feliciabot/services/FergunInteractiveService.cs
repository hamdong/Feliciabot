using Discord.WebSocket;
using Feliciabot.services.interfaces;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using System.Diagnostics.CodeAnalysis;

namespace Feliciabot.services
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
