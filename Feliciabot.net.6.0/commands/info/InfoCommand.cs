using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.services;

namespace Feliciabot.net._6._0.commands
{
    public sealed class InfoCommand(EmbedBuilderService embedBuilderService) : ModuleBase
    {
        [Command("info", RunMode = RunMode.Async)]
        [Summary("Displays legacy information")]
        public async Task Info()
        {
            IGuildUser owner = await Context.Guild.GetOwnerAsync();
            var users = await Context.Guild.GetUsersAsync();

            string botInfo = 
                $"Name: {Context.Client.CurrentUser.Username}\n" +
                $"Created by: Ham#1185\n" +
                $"Framework: Discord.NET C#\n" +
                $"Status: {Context.Client.CurrentUser.Status}\n" +
                $"Currently playing: {Context.Client.CurrentUser.Activities.FirstOrDefault(name => name.ToString() != "")}";

            string serverInfo = 
                $"Name: {Context.Guild.Name}\n" +
                $"Server ID: {Context.Guild.Id}\n" +
                $"Owner: {owner.Username}\n" +
                $"Members: {users.Count}";

            var builder = embedBuilderService.GetBotInfoAsEmbed(botInfo, serverInfo);
            await Context.User.SendMessageAsync("", false, builder);
        }
    }
}
