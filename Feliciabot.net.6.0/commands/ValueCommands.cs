using Discord;
using Discord.Commands;
using System.Security.Cryptography;
using System.Text;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to Fire Emblem class assignment
    /// </summary>
    public class ValueCommands : ModuleBase
    {
        private readonly MD5 md5Hasher;
        public ValueCommands()
        {
            md5Hasher = MD5.Create();
        }

        /// <summary>
        /// Posts the calling user's assigned value
        /// </summary>
        /// <returns>Nothing, posts the user's value</returns>
        [Command("value", RunMode = RunMode.Async)]
        [Summary("Posts the calling user's assigned value. [Usage]: !value")]
        public async Task WhatsMyClass()
        {
            IGuildUser userContext = (IGuildUser)Context.User;

            if(userContext == null)
            {
                await Context.Channel.SendMessageAsync("Unable to find user context in the server");
                return;
            }

            var nickname = userContext.DisplayName;

            if (nickname == null)
            {
                nickname = userContext.Username.ToString();
            }

            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(nickname));
            var ivalue = BitConverter.ToInt32(hashed, 0);

            await Context.Channel.SendMessageAsync("Using name: " + nickname + "; Your value is " + ivalue);
        }
    }
}
