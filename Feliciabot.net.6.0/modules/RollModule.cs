using Discord;
using Discord.Interactions;
using Feliciabot.net._6._0.helpers;
using WaifuSharp;

namespace Feliciabot.net._6._0.modules
{
    public sealed class RollModule : InteractionModuleBase<SocketInteractionContext>
    {
        private const string OOC_CHANNEL_NAME = "out_of_context";
        private readonly WaifuClient _waifuClient;

        private readonly string[][] allReponses = [
            [ "As I see it, yes", "It is certain!", "Most likely!", "Outlook good!", "Signs point to yes", "Without a doubt", "Yes", "Yes - definitely" ],
            [ "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful" ],
            [ "Reply hazy, try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again" ]
            ];

        public RollModule(WaifuClient waifuClient)
        {
            _waifuClient = waifuClient;
        }

        [SlashCommand("8ball", "Answers a question with yes/no/maybe responses", runMode: RunMode.Async)]
        public async Task EightBall(string question)
        {
            int positiveOrNegativeResponse = CommandsHelper.GetRandomNumber(3);
            string[] chosenResponse = allReponses[positiveOrNegativeResponse];
            int randLineIndex = CommandsHelper.GetRandomNumber(chosenResponse.Length - 1);
            await RespondAsync($"Q: {question}\nA: {chosenResponse[randLineIndex]}").ConfigureAwait(false);
        }

        [SlashCommand("roll", "Rolls a 🎲 (default: 6 sided)", runMode: RunMode.Async)]
        public async Task DiceRoll(int sides = 6)
        {
            if (sides <= 0)
            {
                await RespondAsync("Please enter a positive number for the number of sides").ConfigureAwait(false);
                return;
            }

            int randomRoll = CommandsHelper.GetRandomNumber(sides + 1, 1);
            await RespondAsync($"{Context.User.Username} rolled *{randomRoll}*").ConfigureAwait(false);
        }

        [SlashCommand("flip", "Flips a coin", runMode: RunMode.Async)]
        public async Task CoinFlip()
        {
            int headsOrTails = CommandsHelper.GetRandomNumber(2);
            string coinFlipResult = (headsOrTails == 0) ? "Heads" : "Tails";

            await RespondAsync($"{Context.User.Username} got *{coinFlipResult}*").ConfigureAwait(false);
        }

        [SlashCommand("roll-waifu", "Rolls a random waifu from the booru site", runMode: RunMode.Async)]
        public async Task RollWaifu()
        {
            var waifuLink = _waifuClient.GetSfwImage(Endpoints.Sfw.Waifu);
            if (waifuLink is null)
            {
                await RespondAsync("Unable to roll waifu :(").ConfigureAwait(false);
                return;
            }

            await RespondAsync(waifuLink).ConfigureAwait(false);
        }

        [SlashCommand("ooc", "Posts a random image/post from out_of_context (Requires 'out_of_context' channel)", runMode: RunMode.Async)]
        public async Task PostRandomOutOfContext()
        {
            var outOfContextChannel = CommandsHelper.GetChannelByName(Context.Guild, OOC_CHANNEL_NAME);

            if (outOfContextChannel is null)
            {
                await RespondAsync($"No **{OOC_CHANNEL_NAME}** channel found :shrug:").ConfigureAwait(false);
                return;
            }

            try
            {
                await DeferAsync().ConfigureAwait(false);
                IEnumerable<IMessage> oocMessages = await outOfContextChannel.GetMessagesAsync(500).FlattenAsync();

                int maxMsg = oocMessages.Count();
                if (maxMsg == 0)
                {
                    await Context.Channel.SendMessageAsync("Couldn't find a message to post. :confused:");
                    return;
                }

                int randomIndex = CommandsHelper.GetRandomNumber(maxMsg);
                IMessage foundMsg = oocMessages.ElementAt(randomIndex);

                string extractedContents = CommandsHelper.ParseMessageWithAttachments(foundMsg);
                await FollowupAsync($"{extractedContents}").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await FollowupAsync($"Unable to get messages from channel: {outOfContextChannel.Name}").ConfigureAwait(false);
            }
        }

        [SlashCommand("quote-user", "Displays random quotes from users in channel", runMode: RunMode.Async)]
        public async Task QuoteUser(IUser user)
        {
            if (user.IsBot)
            {
                await RespondAsync($"Can't quote bots :shrug:").ConfigureAwait(false);
                return;
            }

            await DeferAsync().ConfigureAwait(false);

            var channelMessages = await Context.Channel.GetMessagesAsync(500).FlattenAsync();
            var userMessages = channelMessages.Where(msg => CommandsHelper.IsNonCommandQuery(msg.Content) && msg.Author == user).ToList();
            var messagesToQuote = SelectRandomMessages(userMessages, 3);
            string formattedMessages = messagesToQuote.Count != 0 ? string.Join(Environment.NewLine, messagesToQuote) : "Couldn't find messages to quote :shrug:";
            await FollowupAsync($"\"{formattedMessages}\"\n-{user.GlobalName}, {messagesToQuote[0].Timestamp.Year}").ConfigureAwait(false);
        }

        [SlashCommand("quote", "Displays random messages or messages that contain specified keyword", runMode: RunMode.Async)]
        public async Task QuoteRandom(string query = "")
        {
            await DeferAsync().ConfigureAwait(false);

            var channelMessages = await Context.Channel.GetMessagesAsync(500).FlattenAsync();
            var userMessages = channelMessages.Where(msg => CommandsHelper.IsNonCommandQuery(msg.Content) && !msg.Author.IsBot && msg.Content.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var messagesToQuote = SelectRandomMessages(userMessages, 3);
            string formattedMessages = messagesToQuote.Count != 0 ? string.Join(Environment.NewLine, messagesToQuote) : "Couldn't find messages to quote :shrug:";
            await FollowupAsync($"{formattedMessages}").ConfigureAwait(false);
        }

        private static List<IMessage> SelectRandomMessages(List<IMessage> messages, int count)
        {
            var rng = new Random();
            var selectedMessages = new List<IMessage>();

            for (int i = 0; i < count && i < messages.Count; i++)
            {
                int randomIndex = rng.Next(messages.Count);
                selectedMessages.Add(messages[randomIndex]);
                messages.RemoveAt(randomIndex); // Remove selected message to avoid duplicates
            }

            return selectedMessages;
        }
    }
}
