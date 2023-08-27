using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.models;
using System.Text.RegularExpressions;

namespace Feliciabot.net._6._0.commands
{
    public class PyradogCommand : ModuleBase
    {
        private static readonly string[] pyraDogArray = {
            EmoteCustom.Pyradog1, EmoteCustom.Pyradog2, EmoteCustom.Pyradog3, EmoteCustom.Pyradog4, EmoteCustom.Pyradog5,
            EmoteCustom.Pyradog6, EmoteCustom.Pyradog7, EmoteCustom.Pyradog8, EmoteCustom.Pyradog9};

        [Command("pyradog", RunMode = RunMode.Async), Summary("Posts Pyradog emote. [Usage] !pyradog")]
        public async Task Pyradog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(pyraDogArray[1]));

        [Alias("tatdog", "tatianadog")]
        [Command("tatidog", RunMode = RunMode.Async), Summary("Posts Tatianadog emote. [Usage] !tatidog, !tatianadog")]
        public async Task Tatianadog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.Tatiana));

        [Command("aibadog", RunMode = RunMode.Async), Summary("Posts Aibadog emote. [Usage] !aibadog")]
        public async Task Aibadog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.Aiba));

        [Command("ninodog", RunMode = RunMode.Async), Summary("Posts Ninodog emote. [Usage] !ninodog")]
        public async Task Ninodog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.Nino));

        [Command("pogdog", RunMode = RunMode.Async), Summary("Posts pogdog emote. [Usage] !pogdog")]
        public async Task Pogdog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.PyraPoggers));

        [Command("okudog", RunMode = RunMode.Async), Summary("Posts okudog emote. [Usage] !okudog")]
        public async Task Okudog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.Oku));

        [Command("cowboyninodog", RunMode = RunMode.Async), Summary("Posts cowboyninodog emote. [Usage] !cowboyninodog")]
        public async Task Cowboyninodog() => await Context.Channel.SendMessageAsync(ConstructPyraDog(EmoteCustom.CowboyNino));

        [Command("shuffledog", RunMode = RunMode.Async), Summary("Posts Pyradog emote in random assortment. [Usage] !shuffledog")]
        public async Task Shuffledog()
        {
            Random rnd = new();
            string[] pyraDogRandom = pyraDogArray.OrderBy(x => rnd.Next()).ToArray();
            await Context.Channel.SendMessageAsync(ConstructPyraDog(pyraDogRandom));
        }

        /// <summary>
        /// Posts Pyradog emote with a randomized head
        /// </summary>
        [Command("randog", RunMode = RunMode.Async), Summary("Posts Pyradog emote with a random emote from the server as the head. [Usage] !randog")]
        public async Task Randog()
        {
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;
            int randomIndex = CommandsHelper.GetRandomNumber(emotes.Count);
            GuildEmote emote = emotes.ElementAt(randomIndex);
            string emoteId = Regex.Match(emote.Url, @"\d+").Value;
            string emoteRef = emote.Name + ":" + emoteId + ">";
            // Determine if the emote is animated
            emoteRef = emote.Animated ? "<a:" + emoteRef : "<:" + emoteRef;

            await Context.Channel.SendMessageAsync(ConstructPyraDog(emoteRef));
        }

        /// <summary>
        /// Gets the PyraDog body emote and appends a head, required to be in an emote reference format
        /// </summary>
        /// <param name="pyradogHead">Reference to the id of the emote to make the head</param>
        /// <returns>Returns the entire Pyradog emote with a custom head</returns>
        private static string ConstructPyraDog(string pyradogHead)
        {
            return $"{pyraDogArray[0]}{pyradogHead}{pyraDogArray[2]}\n" +
                $"{pyraDogArray[3]}{pyraDogArray[4]}{pyraDogArray[5]}\n" +
                $"{pyraDogArray[6]}{pyraDogArray[7]}{pyraDogArray[8]}";
        }

        /// <summary>
        /// Gets the PyraDog body emote based on the passed array of pieces
        /// </summary>
        /// <param name="pyradogPieces">Reference to the pieces of the emote as an array</param>
        /// <returns>Returns the entire Pyradog emote</returns>
        private static string ConstructPyraDog(string[] pyradogPieces)
        {
            return $"{pyradogPieces[0]}{pyradogPieces[1]}{pyradogPieces[2]}\n" +
                $"{pyradogPieces[3]}{pyradogPieces[4]}{pyradogPieces[5]}\n" +
                $"{pyradogPieces[6]}{pyradogPieces[7]}{pyradogPieces[8]}";
        }
    }
}
