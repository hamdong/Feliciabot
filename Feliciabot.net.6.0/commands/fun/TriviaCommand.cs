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
        [Command("pin", RunMode = RunMode.Async)]
        [Summary("Displays random pinned channel text message. [Usage]: !pin")]
        public async Task Pin()
        {
            IEnumerable<IMessage> pinnedMessages = await Context.Channel.GetPinnedMessagesAsync();
            IMessage[] pinnedMessagesArray = pinnedMessages.ToArray();
            IMessage[] culledMessagesArray = CommandsHelper.CullEmptyContentFromMessageList(pinnedMessagesArray.ToArray());

            if (!culledMessagesArray.Any())
            {
                await Context.Channel.SendMessageAsync("Couldn't find a pinned text message. :confused:");
                return;
            }

            int randIndex = CommandsHelper.GetRandomNumber(culledMessagesArray.Length);
            await Context.Channel.SendMessageAsync(culledMessagesArray[randIndex].Content);
        }
    }
}
