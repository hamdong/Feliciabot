using Discord.Interactions;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.modules
{
    public class HelpModule(IPaginatorService paginatorService) : InteractionModuleBase
    {
        [SlashCommand("help_info", "Lists info commands", runMode: RunMode.Async)]
        public async Task HelpInfo()
        {
            await PostHelpInteraction("Info");
        }

        [SlashCommand("help_music", "Lists music commands", runMode: RunMode.Async)]
        public async Task HelpMusic()
        {
            await PostHelpInteraction("Music");
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
            var paginator = paginatorService.BuildModulesPaginator(Context, moduleName);
            await RespondAsync($"{moduleName} Commands").ConfigureAwait(false);
            await paginatorService.SendPaginatorAsync(Context, paginator);
        }
    }
}
