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
            string botInfo =
                $"Name: {clientService.GetUsername()}\n" +
                $"Created by: Ham#1185\n" +
                $"Framework: Discord.NET C#\n" +
                $"Status: {clientService.GetStatus()}\n" +
                $"Currently playing: {clientService.GetActivities().FirstOrDefault(name => name.ToString() != "")}\n" +
                $"Currently in: {clientService.GetGuildCount()} servers!";

            var builder = EmbedBuilderService.GetBotInfoAsEmbed(botInfo);
            await interactingService.SendResponseToUserAsync(Context, builder).ConfigureAwait(false);
        }
    }
}
