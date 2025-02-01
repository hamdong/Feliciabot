using Discord.Commands;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.commands
{
    public class HelpCommand(CommandService _service, InteractiveService _interactiveService) : ModuleBase
    {
        private const int NUM_ITEMS_PER_PAGE = 12;

        [Command("icanhelp", RunMode = RunMode.Async)]
        [Summary("Lists all commands in an embedded paginator")]
        public async Task ICanHelp()
        {
            List<string> pageList = [];
            string pageContent = string.Empty;
            int itemCount = 1;
            var groupedCommands = _service.Commands.GroupBy(c => c.Name).Select(g => g.First()).ToList();

            if (groupedCommands.Count == 0)
            {
                await Context.Channel.SendMessageAsync("Can't find any commands :confused:");
                return;
            }

            foreach (CommandInfo command in groupedCommands)
            {
                pageContent += ($"!**{command.Name}**: {command.Summary}\n");
                if (itemCount % NUM_ITEMS_PER_PAGE == 0)
                {
                    pageList.Add(pageContent);
                    pageContent = string.Empty;
                }
                itemCount++;
            }

            if (pageContent != string.Empty)
            {
                pageList.Add(pageContent);
            }

            // Create paginated message
            var pages = pageList.ToArray();
            List<PageBuilder> pagebuilder = [];

            foreach (string page in pages)
            {
                pagebuilder.Add(new PageBuilder().WithDescription(page));
            }

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User)  // Only interacted user can parse pages
                .WithPages(pagebuilder)
                .Build();

            await _interactiveService.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(5));
        }
    }
}
