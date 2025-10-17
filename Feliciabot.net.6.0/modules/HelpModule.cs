using Discord.Interactions;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.modules
{
    public class HelpModule(IInteractiveHelperService interactiveHelperService)
        : InteractionModuleBase
    {
        [SlashCommand("help_info", "Lists info commands", runMode: RunMode.Async)]
        public async Task HelpInfo()
        {
            await PostHelpInteraction("Info");
        }

        [SlashCommand("help_roleplay", "Lists roleplay commands", runMode: RunMode.Async)]
        public async Task HelpRolePlay()
        {
            await PostHelpInteraction("RolePlay");
        }

        [SlashCommand("help_roll", "Lists roll commands", runMode: RunMode.Async)]
        public async Task HelpRoll()
        {
            await PostHelpInteraction("Roll");
        }

        private async Task PostHelpInteraction(string moduleName)
        {
            List<string> commandList = [];
            string pageContent = string.Empty;
            var modules = interactiveHelperService.GetNonHelpSlashCommandsByName(moduleName);

            if (modules.Length == 0)
            {
                await RespondAsync($"No modules found for '{moduleName}'").ConfigureAwait(false);
                return;
            }

            foreach (var (Name, Description) in modules)
            {
                pageContent += ($"**/{Name}**: {Description}\n");
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

            // Only the context user can interact
            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User)
                .WithPages(pagebuilder)
                .Build();

            await RespondAsync($"{moduleName} Commands").ConfigureAwait(false);
            await interactiveHelperService.SendPaginatorAsync(Context, paginator);
        }
    }
}
