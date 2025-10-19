using Discord.Interactions;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.modules
{
    public class InfoModule(IEmbedBuilderService _embedBuilderService) : InteractionModuleBase
    {
        [SlashCommand("info", "Displays legacy information", runMode: RunMode.Async)]
        public async Task Info()
        {
            var client = Context.Client.CurrentUser;
            var guilds = await Context.Client.GetGuildsAsync();
            string botInfo =
                $"Name: {client.Username}\n"
                + $"Created by: Ham\n"
                + $"Framework: Discord.NET C#\n"
                + $"Status: {client.Status}\n"
                + $"Currently playing: {client.Activities.FirstOrDefault(name => name.ToString() != "")}\n"
                + $"Currently in: {guilds.Count} servers!";

            var builder = _embedBuilderService.GetBotInfoAsEmbed(botInfo);
            await RespondAsync(embed: builder).ConfigureAwait(false);
        }
    }
}
