using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.models;
using System.Xml.Linq;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to rolling
    /// </summary>
    public class RollCommand : ModuleBase
    {
        private const string OUTOFCONTEXT_CHANNEL_NAME = "out_of_context";
        private readonly string waifuListPath = Environment.CurrentDirectory + @"\data\waifus.xml";
        private readonly string husbandosListPath = Environment.CurrentDirectory + @"\data\husbandos.xml";

        private readonly Waifu[] waifuList;
        private readonly Waifu[] husbandoList;

        public RollCommand()
        {
            waifuList = GetListToWaifuArray(waifuListPath);
            husbandoList = GetListToWaifuArray(husbandosListPath);
        }

        /// <summary>
        /// Rolls the dice based on a specified max value
        /// </summary>
        /// <param name="max">max number that can be rolled on the dice</param>
        /// <returns>Nothing, post the dice roll result in the channel</returns>
        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Felicia will roll the dice for you. If no dice type specified then defaults to 6. [Usage]: !roll 6, !dice")]
        [Alias("dice")]
        public async Task DiceRoll([Summary("Max number.")] int max = 0)
        {
            int randomRoll;

            // If not max specified then just default to 6-sided die
            if (max != 0) randomRoll = CommandsHelper.GetRandomNumber(max + 1, 1);
            else randomRoll = CommandsHelper.GetRandomNumber(7, 1);

            await Context.Channel.SendMessageAsync(Context.Message.Author.Username + " rolled *" + randomRoll + "*");
        }

        /// <summary>
        /// Flips a coin and posts the result
        /// </summary>
        /// <returns>Nothing, posts the coin flip result in the channel</returns>
        [Command("flip", RunMode = RunMode.Async)]
        [Summary("Felicia will flip a coin for you. [Usage]: !flip")]
        public async Task CoinFlip()
        {
            int headsOrTails = CommandsHelper.GetRandomNumber(2);
            string coinFlipResult = (headsOrTails == 0) ? "Heads" : "Tails";

            await Context.Channel.SendMessageAsync(Context.Message.Author.Username + " got *" + coinFlipResult + "*");
        }

        /// <summary>
        /// Posts a random message from the #out_of_context channel
        /// </summary>
        /// <returns>Nothing, posts a random message from the #out_of_context channel</returns>
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
                IEnumerable<IMessage> oocMessages = await outOfContextChannel.GetMessagesAsync(200).FlattenAsync();
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

        /// <summary>
        /// Roll husbando command
        /// </summary>
        /// <returns></returns>
        [Command("rollhusbando", RunMode = RunMode.Async)]
        [Summary("Felicia will roll you a husbando. [Usage]: !rollhusbando")]
        public async Task RollHusbando()
        {
            await RollFromList(husbandoList);
        }

        /// <summary>
        /// Roll waifu command
        /// </summary>
        /// <returns></returns>
        [Command("rollwaifu", RunMode = RunMode.Async)]
        [Summary("Felicia will roll you a waifu. [Usage]: !rollwaifu")]
        public async Task RollWaifu()
        {
            await RollFromList(waifuList);
        }

        /// <summary>
        /// Gets a random waifu from the array
        /// </summary>
        /// <param name="waifuOrHusbandoList">List of husbandos or waifus</param>
        /// <returns>Waifu to post</returns>
        private async Task RollFromList(Waifu[] waifuOrHusbandoList)
        {
            int randIndex = CommandsHelper.GetRandomNumber(waifuOrHusbandoList.Length);
            string name = waifuOrHusbandoList[randIndex].Name;
            string series = waifuOrHusbandoList[randIndex].Series ?? "N/A";
            string pic = waifuOrHusbandoList[randIndex].Pic ?? "";
            string link = waifuOrHusbandoList[randIndex].Link ?? "N/A";

            await PostWaifu(name, series, pic, link);
        }

        /// <summary>
        /// Posts the specified waifu in the channel
        /// </summary>
        /// <param name="name">name of the waifu</param>
        /// <param name="series">series the waifu is from</param>
        /// <param name="pic">image link to the waifu</param>
        /// <param name="link">link to source of waifu</param>
        /// <returns>Nothing, posts the waifu in the channel</returns>
        private async Task PostWaifu(string name, string series, string pic, string link)
        {
            string response;
            if (name == "Felicia")
            {
                response = ":tada: " + Context.Message.Author.Username + " rolled *" + name + "* :tada:" + "*\nSeries: *" + series + "*";
                await Context.Channel.SendMessageAsync(response);
            }
            else if (name == "Wrys")
            {
                response = Context.Message.Author.Username + " rolled *" + name + "* :sunglasses:" + "*\nSeries: *" + series + "*";
                await Context.Channel.SendMessageAsync(response);
            }
            else
            {
                response = Context.Message.Author.Username + " rolled *" + name + "*\nSeries: *" + series + "*";
                await Context.Channel.SendMessageAsync(response);
            }

            var builder = new EmbedBuilder();
            builder.WithImageUrl(pic);
            builder.WithDescription("[" + name + "](" + link + ")");
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        /// <summary>
        /// Converts a waifu block from the xml file into an array
        /// </summary>
        /// <param name="file">path to the xml file</param>
        /// <returns>Array of waifus</returns>
        private static Waifu[] GetListToWaifuArray(string file)
        {
            try
            {
                var doc = XElement.Load(file);
                var waifus = doc.Descendants("waifu")
                    .Select(s => new Waifu
                    {
                        Name = s.Element("name").Value,
                        Series = s.Element("series").Value,
                        Pic = s.Element("pic").Value,
                        Link = s.Element("link").Value
                    }).ToArray();
                return waifus;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Waifu file not found");
                return Array.Empty<Waifu>();
            }
        }
    }
}
