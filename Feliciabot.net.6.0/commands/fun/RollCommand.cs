using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class RollCommand : ModuleBase
    {
        private const string OUTOFCONTEXT_CHANNEL_NAME = "out_of_context";
        private readonly int MAX_MESSAGE_SEARCH = 500;

        /// <summary>
        /// Posts a random message from the #out_of_context channel
        /// </summary>
        [Command("ooc", RunMode = RunMode.Async)]
        [Summary("Felicia will post a random image/post from out_of_context. Requires channel named out_of_context. [Usage]: !ooc")]
        public async Task CheckRandomOutOfContext()
        {
            var outOfContextChannel = CommandsHelper.GetChannelByName((SocketGuild)Context.Guild, OUTOFCONTEXT_CHANNEL_NAME);

            // Determine if the channel exists
            if (outOfContextChannel is null)
            {
                // Find a random channel if none exists
                IReadOnlyCollection<SocketTextChannel> channels = ((SocketGuild)Context.Guild).TextChannels;
                channels = channels.Where(x => !x.IsNsfw).ToList();

                if (channels.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("No **#" + OUTOFCONTEXT_CHANNEL_NAME +
                        "** channel or accessible channels found. Please create a text channel called **#" + OUTOFCONTEXT_CHANNEL_NAME +
                        "** in order to use this command.");
                    return;
                }
                else
                {
                    int randomChannelIndex = CommandsHelper.GetRandomNumber(channels.Count);
                    outOfContextChannel = channels.ElementAt(randomChannelIndex);
                }
            }

            try
            {
                IEnumerable<IMessage> oocMessages = await outOfContextChannel.GetMessagesAsync(MAX_MESSAGE_SEARCH).FlattenAsync();
                // Determine if there are messages in the channel
                int maxMsg = oocMessages.Count();
                if (maxMsg == 0)
                {
                    await Context.Channel.SendMessageAsync("Couldn't find a message to post. :confused:");
                    return;
                }

                // Get random message from channel
                int randomIndex = CommandsHelper.GetRandomNumber(maxMsg);
                IMessage foundMsg = oocMessages.ElementAt(randomIndex);

                // Post message contents with random attachment
                string extractedContents = CommandsHelper.GetMessageWithRandomAttachment(foundMsg);
                await Context.Channel.SendMessageAsync(extractedContents);
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("Unable to get messages from found channel: " + outOfContextChannel.Name);
            }
        }
    }
}
