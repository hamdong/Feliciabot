using Discord.Interactions;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.modules
{
    public class InfoModule(IClientService clientService, IInteractingService interactingService) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("info", "Displays legacy information", runMode: RunMode.Async)]
        public async Task Info()
        {
            var client = clientService.GetClient();
            string botInfo =
                $"Name: {client.Username}\n" +
                $"Created by: Ham#1185\n" +
                $"Framework: Discord.NET C#\n" +
                $"Status: {client.Status}\n" +
                $"Currently playing: {client.Activities.FirstOrDefault(name => name.ToString() != "")}\n" +
                $"Currently in: {client.Guilds.Count} servers!";

            var builder = EmbedBuilderService.GetBotInfoAsEmbed(botInfo);
            await interactingService.SendResponseToUserAsync(Context, builder).ConfigureAwait(false);
        }
    }
}
