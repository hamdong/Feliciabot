using Discord;
using Fergun.Interactive.Pagination;

namespace Feliciabot.services.interfaces
{
    public interface IInteractiveHelperService
    {
        public (string Name, string Description)[] GetNonHelpSlashCommandsByName(string query);
        public Task SendPaginatorAsync(IInteractionContext context, StaticPaginator paginator);
    }
}
