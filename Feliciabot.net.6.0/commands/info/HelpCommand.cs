using Discord.Commands;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to help and bot information
    /// </summary>
    public class HelpCommand : ModuleBase
    {
        private const int NUM_ITEMS_PER_PAGE = 10;

        private readonly string[] omittedCommands = {
            "feliciabotdelete", "feliciabotclean", "feliciabotleave", "thor", "boris"
        };
        public static List<string> commands = new List<string>();
        private readonly CommandService _service;
        private readonly InteractiveService _interactiveService;

        public HelpCommand(CommandService service, InteractiveService interactiveService)
        {
            _service = service;
            _interactiveService = interactiveService;
        }

        /// <summary>
        /// Posts the list of commands and their functions
        /// </summary>
        /// <returns></returns>
        [Command("icanhelp", RunMode = RunMode.Async)]
        [Summary("Lists all commands in an embedded paginator. [Usage]: !icanhelp")]
        public async Task ICanHelp()
        {
            List<string> trackList = new List<string>();
            string lastCommandAdded = string.Empty;
            string pageContent = string.Empty;
            int itemOnPageCount = 1;
            foreach (CommandInfo command in _service.Commands.ToList())
            {
                if (!omittedCommands.Contains(command.Name) && lastCommandAdded != command.Name)
                {
                    pageContent += ($"!**{command.Name}**\n{command.Summary}\n\n");
                    if (itemOnPageCount % NUM_ITEMS_PER_PAGE != 0)
                    {
                        itemOnPageCount++;
                    }
                    else
                    {
                        trackList.Add(pageContent);
                        pageContent = string.Empty;
                        itemOnPageCount = 1;
                    }
                    lastCommandAdded = command.Name;
                }
            }

            if (pageContent != string.Empty)
            {
                trackList.Add(pageContent);
            }

            // Create paginated message
            var pages = trackList.ToArray();
            List<PageBuilder> pagebuilder = new List<PageBuilder>();

            foreach(string page in pages)
            {
                pagebuilder.Add(new PageBuilder().WithDescription(page));
            }

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only allow the user that executed the command to interact with the selection.
                .WithPages(pagebuilder) // Set the pages the paginator will use. This is the only required component.
                .Build();

            // Send the paginator to the source channel and wait until it times out after 10 minutes.
            await _interactiveService.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));
        }
    }
}
