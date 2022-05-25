using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to getting trivia
    /// </summary>
    public class TriviaCommand : ModuleBase
    {
        private readonly string[] triviasList;

        private readonly string triviasPath = Environment.CurrentDirectory + @"\data\trivias.txt";

        /// <summary>
        /// Constructor for initializers
        /// </summary>
        public TriviaCommand()
        {
            triviasList = File.ReadAllLines(triviasPath);
        }

        /// <summary>
        /// Displays random trivia
        /// </summary>
        /// <returns>Post trivia to message channel</returns>
        [Command("trivia", RunMode = RunMode.Async)]
        [Alias("triv")]
        [Summary("Displays random trivia. [Usage]: !trivia, !triv")]
        public async Task Trivia()
        {
            int randIndex = CommandsHelper.GetRandomNumber(triviasList.Length);
            string triviaToPost = triviasList[randIndex];
            await Context.Channel.SendMessageAsync(triviaToPost);
        }

        /// <summary>
        /// Gets a random pinned message from the list of pins in the current channel
        /// </summary>
        /// <returns>Posts random pinned test message in the channel</returns>
        [Command("pin", RunMode = RunMode.Async)]
        [Summary("Displays random pinned channel text message. [Usage]: !pin")]
        public async Task Pin()
        {
            // Get all pinned messages
            IEnumerable<IMessage> pinnedMessages = await Context.Channel.GetPinnedMessagesAsync();

            // Convert to array
            IMessage[] pinnedMessagesArray = pinnedMessages.ToArray();
            IMessage[] culledMessagesArray = CommandsHelper.CullEmptyContentFromMessageList(pinnedMessagesArray);

            // Check if any text messages exist
            if (culledMessagesArray.Count() > 0)
            {
                // Pick a random index from the array
                int randIndex = CommandsHelper.GetRandomNumber(culledMessagesArray.Count());

                // Display message
                await Context.Channel.SendMessageAsync(culledMessagesArray[randIndex].Content);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Couldn't find a pinned text message. :confused:");
            }
        }
    }
}
