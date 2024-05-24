using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class QuoteCommand : ModuleBase
    {
        private const int DELAY_BEFORE_DELETE_BOT_MSG = 3000;

        /// <summary>
        /// Displays random quotes from users in channel or quotes that contain a specified keyword
        /// </summary>
        /// <param name="searchQuery">User to quote if specified</param>
        [Command("quote", RunMode = RunMode.Async)]
        [Summary("Displays random quotes from users in channel or quotes that contain a specified keyword. [Usage]: !quote @user/keyword")]
        public async Task Quote(string searchQuery = "")
        {
            const int MAX_MESSAGES_DOWNLOAD = 500;
            IUser? mentionedUser = null;

            // Get first mentioned user if applicable
            if (Context.Message.MentionedUserIds.Count > 0)
            {
                ulong mentionedUserId = Context.Message.MentionedUserIds.ElementAt(0);
                mentionedUser = await Context.Guild.GetUserAsync(mentionedUserId);

                // Don't quote bot users
                if (mentionedUser.IsBot)
                {
                    var m = await this.ReplyAsync($"Can't quote bots. :shrug: This message will be auto-deleted.");
                    await Task.Delay(DELAY_BEFORE_DELETE_BOT_MSG);
                    await m.DeleteAsync();
                    return;
                }
            }

            // Get collection of messages from mentioned user or contains query string
            IEnumerable<IMessage> messagesInChannel;
            messagesInChannel = await Context.Channel.GetMessagesAsync(MAX_MESSAGES_DOWNLOAD).FlattenAsync();
            List<IMessage> messagesFromQueryReturn = CommandsHelper.GetMessagesMatchingQueryParameters(messagesInChannel, searchQuery, mentionedUser);

            // Get messages to quote and post to channel
            string messagesToQuote = CommandsHelper.GetMessagesToQuote(messagesFromQueryReturn, mentionedUser);
            await Context.Channel.SendMessageAsync(messagesToQuote);
        }
    }
}
