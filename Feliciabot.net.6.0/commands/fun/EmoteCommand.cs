using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Feliciabot.net._6._0.commands
{
    public class EmoteCommand(IMessagingService _msgingService) : ModuleBase<ICommandContext>
    {
        private const int MAX_CLAPS = 12;
        private static readonly string[] pyraDogArray = [
            EmoteCustom.Pyradog1, EmoteCustom.Pyradog2, EmoteCustom.Pyradog3, EmoteCustom.Pyradog4, EmoteCustom.Pyradog5,
            EmoteCustom.Pyradog6, EmoteCustom.Pyradog7, EmoteCustom.Pyradog8, EmoteCustom.Pyradog9];

        [Command("civ", RunMode = RunMode.Async), Summary("Posts Feliciaciv emote")]
        public async Task Civ() => await Context.Channel.SendMessageAsync(EmoteCustom.FeliciaCiv);

        [Command("pad", RunMode = RunMode.Async), Summary("Posts Feliciadoru emote")]
        public async Task Padoru() => await _msgingService.SendMessageToContextAsync(Context, EmoteCustom.Padoru);

        [Command("sip", RunMode = RunMode.Async), Summary("Posts Pyrasip emote")]
        public async Task Sip() => await _msgingService.SendMessageToContextAsync(Context, EmoteCustom.PyraSip);

        [Command("spin", RunMode = RunMode.Async), Summary("Posts Feliciaspin emote")]
        public async Task Spin() => await _msgingService.SendMessageToContextAsync(Context, EmoteCustom.FeliciaSpin);

        [Command("clap", RunMode = RunMode.Async), Summary("Posts Wiiclap emote")]
        public async Task Clap() => await Clap(1);

        [Command("clap", RunMode = RunMode.Async), Summary("Posts Wiiclap emote (max 12)")]
        public async Task Clap(int copies)
        {
            copies = copies > MAX_CLAPS ? MAX_CLAPS : copies;
            StringBuilder sb = new();

            for (int i = 0; i < copies; i++)
            {
                sb.Append(EmoteCustom.WiiClap);
            }
            await _msgingService.SendMessageToContextAsync(Context, sb.ToString());
        }

        [Command("pyradog", RunMode = RunMode.Async), Summary("Posts Pyradog emote")]
        public async Task Pyradog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(pyraDogArray[1]));

        [Alias("tatdog", "tatianadog")]
        [Command("tatidog", RunMode = RunMode.Async), Summary("Posts Tatianadog emote")]
        public async Task Tatianadog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.Tatiana));

        [Command("aibadog", RunMode = RunMode.Async), Summary("Posts Aibadog emote")]
        public async Task Aibadog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.Aiba));

        [Command("ninodog", RunMode = RunMode.Async), Summary("Posts Ninodog emote")]
        public async Task Ninodog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.Nino));

        [Command("pogdog", RunMode = RunMode.Async), Summary("Posts pogdog emote")]
        public async Task Pogdog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.PyraPoggers));

        [Command("okudog", RunMode = RunMode.Async), Summary("Posts okudog emote")]
        public async Task Okudog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.Oku));

        [Command("cowboyninodog", RunMode = RunMode.Async), Summary("Posts cowboyninodog emote")]
        public async Task Cowboyninodog() => await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(EmoteCustom.CowboyNino));

        [Command("shuffledog", RunMode = RunMode.Async), Summary("Posts Pyradog emote in random assortment")]
        public async Task Shuffledog()
        {
            Random rnd = new();
            string[] pyraDogRandom = [.. pyraDogArray.OrderBy(x => rnd.Next())];
            await _msgingService.SendMessageToContextAsync(Context, ConstructPyraDog(pyraDogRandom));
        }

        [Command("randog", RunMode = RunMode.Async), Summary("Posts Pyradog emote with a random emote from the server as the head")]
        public async Task Randog()
        {
            var context = Context;
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;
            int randomIndex = CommandsHelper.GetRandomNumber(emotes.Count);
            GuildEmote emote = emotes.ElementAt(randomIndex);
            string emoteId = Regex.Match(emote.Url, @"\d+").Value;
            string emoteRef = emote.Name + ":" + emoteId + ">";
            // Determine if the emote is animated
            emoteRef = emote.Animated ? "<a:" + emoteRef : "<:" + emoteRef;

            await _msgingService.SendMessageToContextAsync(context, ConstructPyraDog(emoteRef));
        }

        private static string ConstructPyraDog(string pyradogHead)
        {
            return $"{pyraDogArray[0]}{pyradogHead}{pyraDogArray[2]}\n" +
                $"{pyraDogArray[3]}{pyraDogArray[4]}{pyraDogArray[5]}\n" +
                $"{pyraDogArray[6]}{pyraDogArray[7]}{pyraDogArray[8]}";
        }

        private static string ConstructPyraDog(string[] pyradogPieces)
        {
            return $"{pyradogPieces[0]}{pyradogPieces[1]}{pyradogPieces[2]}\n" +
                $"{pyradogPieces[3]}{pyradogPieces[4]}{pyradogPieces[5]}\n" +
                $"{pyradogPieces[6]}{pyradogPieces[7]}{pyradogPieces[8]}";
        }
    }
}
