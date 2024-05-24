using Discord;
using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    public class OkayCommand : ModuleBase
    {
        /// <summary>
        /// Posts ok with the previous member's username or a specified username
        /// </summary>
        [Command("ok", RunMode = RunMode.Async)]
        [Summary("Posts ok [previous member username]. [Usage] !ok")]
        public async Task Ok()
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(50).FlattenAsync();
            string callOutUser = Context.Message.Author.Username;

            foreach (IMessage m in messages)
            {
                if (m.Author != Context.Message.Author)
                {
                    callOutUser = m.Author.Username;
                    break;
                }
            }

            await Context.Channel.SendMessageAsync("ok " + callOutUser);
        }

        /// <summary>
        /// Overloaded method, posts ok with the mentioned username
        /// </summary>
        /// <param name="user">User to call out</param>
        [Command("ok", RunMode = RunMode.Async)]
        [Summary("Posts ok [mentioned member username]. [Usage] !ok @member")]
        public async Task Ok(IUser user) => await Context.Channel.SendMessageAsync("ok " + user.Username);
    }
}
