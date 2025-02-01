using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services
{
    [ExcludeFromCodeCoverage]
    public class InteractiveHelperService(
        InteractiveService interactiveService,
        InteractionService interactionService
    ) : InteractionModuleBase<SocketInteractionContext>, IInteractiveHelperService
    {
        public (string Name, string Description)[] GetNonHelpSlashCommandsByName(string query)
        {
            return interactionService
                .SlashCommands.Where(c =>
                    c.Module.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                    && !c.Name.Contains("help_")
                )
                .OrderBy(c => c.Name)
                .Select(c => (c.Name, c.Description))
                .ToArray();
        }

        public async Task SendPaginatorAsync(IInteractionContext context, StaticPaginator paginator)
        {
            await interactiveService
                .SendPaginatorAsync(paginator, context.Channel, TimeSpan.FromMinutes(5))
                .ConfigureAwait(false);
        }
    }
}
