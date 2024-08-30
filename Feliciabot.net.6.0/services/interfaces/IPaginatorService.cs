using Discord;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IPaginatorService
    {
        public StaticPaginator BuildModulesPaginator(IInteractionContext context, string moduleName);
        public Task SendPaginatorAsync(IInteractionContext context, StaticPaginator paginator);
    }
}
