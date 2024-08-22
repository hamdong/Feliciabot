using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.services
{
    public class PaginatorService(
        InteractionService interactionService,
        InteractiveService interactiveService
    ) : IPaginatorService
    {
        public StaticPaginator BuildPaginator(SocketInteractionContext<SocketInteraction> context, string moduleName)
        {
            List<string> commandList = [];
            string pageContent = string.Empty;
            var moduleCollection = interactionService
                .SlashCommands.Where(c => c.Module.Name.Contains(moduleName))
                .OrderBy(c => c.Name);

            foreach (var command in moduleCollection)
            {
                pageContent += ($"**/{command.Name}**: {command.Description}\n");
                if ((pageContent.Split('\n').Length - 1) % 12 == 0)
                {
                    commandList.Add(pageContent);
                    pageContent = string.Empty;
                }
            }

            // Add the remaining commands if not evenly divisible
            if (pageContent != string.Empty)
            {
                commandList.Add(pageContent);
            }

            var pages = commandList.ToArray();
            List<PageBuilder> pagebuilder = [];

            foreach (string page in pages)
            {
                pagebuilder.Add(new PageBuilder().WithDescription(page));
            }

            // Only the user that executed can interact
            return new StaticPaginatorBuilder()
                .AddUser(context.User)
                .WithPages(pagebuilder)
                .Build();
        }

        public async Task SendPaginatorAsync(SocketInteractionContext<SocketInteraction> context, StaticPaginator paginator)
        {
            await interactiveService.SendPaginatorAsync(
                paginator,
                context.Channel,
                TimeSpan.FromMinutes(5)
            );
        }
    }
}
