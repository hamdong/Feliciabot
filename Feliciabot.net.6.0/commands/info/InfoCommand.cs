using Discord;
using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to bot information
    /// </summary>
    public class InfoCommand : ModuleBase
    {
        /// <summary>
        /// Shows legacy information on the bot
        /// </summary>
        /// <returns></returns>
        [Command("info", RunMode = RunMode.Async)]
        [Summary("Displays legacy information on Feliciabot. [Usage]: !info")]
        public async Task Info()
        {
            IGuildUser owner = await Context.Guild.GetOwnerAsync();
            var users = await Context.Guild.GetUsersAsync();

            string infoText = "Name: " + Context.Client.CurrentUser.Username + "\n" +
              "Created by: Ham#1185" + "\n" +
              "Framework: Discord.NET C#\n" +
              "Status: " + Context.Client.CurrentUser.Status + "\n" +
              "Currently playing: " + Context.Client.CurrentUser.Activities.FirstOrDefault(name => name.ToString() != "");

            string serverText = "Name: " + Context.Guild.Name + "\n" +
                "Server ID: " + Context.Guild.Id.ToString() + "\n" +
                "Owner: " + owner.Username + "\n" +
                "Members: " + users.Count.ToString();

            var builder = new EmbedBuilder();

            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", infoText);
            builder.AddField("Server Info", serverText);

            builder.WithThumbnailUrl("https://raw.githubusercontent.com/Andu2/FEH-Mass-Simulator/master/heroes/Felicia.png");
            builder.WithColor(Color.LightGrey);
            await Context.User.SendMessageAsync("", false, builder.Build());

        }
    }
}
