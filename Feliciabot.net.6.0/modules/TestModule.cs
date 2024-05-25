using Discord.Interactions;
using Feliciabot.net._6._0.services;

namespace Feliciabot.net._6._0.modules
{
    public sealed class TestModule(EmbedBuilderService embedBuilderService) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("test", description: "Testing slash command.", runMode: RunMode.Async)]
        public async Task TestAsync()
        {
            await RespondAsync("Test Success!").ConfigureAwait(false);
        }

        [SlashCommand("embed", description: "Testing embeds.", runMode: RunMode.Async)]
        public async Task EmbedAsync()
        {
            await RespondAsync(embed: embedBuilderService.GetTestEmbed()).ConfigureAwait(false);
        }
    }
}
