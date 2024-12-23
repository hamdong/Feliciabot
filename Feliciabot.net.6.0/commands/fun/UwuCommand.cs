using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Uwuify.Humanizer;

namespace Feliciabot.net._6._0.commands
{
    public class UwuCommand : ModuleBase
    {

        [Command("uwu", RunMode = RunMode.Async)]
        [Alias("uwufy", "uwuify")]
        [Summary("Posts uwuified message of last message")]
        public async Task Uwu()
        {
            IEnumerable<IMessage> messages;
            messages = await Context.Channel.GetMessagesAsync(50).FlattenAsync();

            // Get the last message from the user who posted
            string foundMessage = "No messages found uwu.";
            foreach (IMessage m in messages)
            {
                if (!m.Author.IsBot && CommandsHelper.IsNonCommandQuery(m.Content) && !CommandsHelper.IsEmoteMessage(m.Content))
                {
                    foundMessage = m.Content;
                    break;
                }
            }

            string response = foundMessage.Uwuify();
            await Context.Channel.SendMessageAsync(response);
        }

        [Command("uwu", RunMode = RunMode.Async)]
        [Alias("uwufy", "uwuify")]
        [Summary("Posts uwuified message of following message")]
        public async Task Uwu([Remainder] string message)
        {
            string response = message.Uwuify();
            await Context.Channel.SendMessageAsync(response);
        }
    }
}
