using Discord;
using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to calling out ok
    /// </summary>
    public class OkayCommand : ModuleBase
    {
        /// <summary>
        /// Posts ok with the previous member's username or a specified username
        /// </summary>
        /// <returns>Nothing, just posts ok</returns>
        [Command("ok", RunMode = RunMode.Async)]
        [Summary("Posts ok [previous member username]. [Usage] !ok")]
        public async Task Ok()
        {
            // Get last batch of messages
            IEnumerable<IMessage> messages;
            messages = await Context.Channel.GetMessagesAsync(50).FlattenAsync();

            // Get the last user who posted
            string callOutUser = Context.Message.Author.Username;
            foreach (IMessage m in messages)
            {
                if (m.Author != Context.Message.Author)
                {
                    callOutUser = m.Author.Username;
                    break;
                }
            }

            // Call out
            await Context.Channel.SendMessageAsync("ok " + callOutUser);
        }

        /// <summary>
        /// Overloaded method, posts ok with the mentioned username
        /// </summary>
        /// <param name="user">User to call out</param>
        /// <returns>Nothing, just posts ok</returns>
        [Command("ok", RunMode = RunMode.Async)]
        [Summary("Posts ok [mentioned member username]. [Usage] !ok @member")]
        public async Task Ok(IUser user)
        {
            await Context.Channel.SendMessageAsync("ok " + user.Username);
        }
    }
}
