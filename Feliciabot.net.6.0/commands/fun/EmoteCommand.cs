using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to posting emotes
    /// </summary>
    public class EmoteCommand : ModuleBase
    {
        private const int MAX_CLAPS = 12;

        [Command("civ", RunMode = RunMode.Async), Summary("Posts Feliciaciv emote. [Usage] !civ")]
        public async Task Civ()
        {
            await Context.Channel.SendMessageAsync("<a:feliciaciv:547067697301553154>");
        }

        [Command("pad", RunMode = RunMode.Async), Summary("Posts Feliciadoru emote. [Usage] !pad")]
        public async Task Padoru()
        {
            await Context.Channel.SendMessageAsync("<a:padoru:547086860438994944>");
        }

        [Command("sip", RunMode = RunMode.Async), Summary("Posts Pyrasip emote. [Usage] !sip")]
        public async Task Sip()
        {
            await Context.Channel.SendMessageAsync("<:pyrasip:558098152083685394>");
        }

        [Command("spin", RunMode = RunMode.Async), Summary("Posts Feliciaspin emote. [Usage] !spin")]
        public async Task Spin()
        {
            await Context.Channel.SendMessageAsync("<a:spin:596160078487355393>");
        }

        [Command("clap", RunMode = RunMode.Async), Summary("Posts wiiclap emote. Max claps 12. [Usage] !clap [number of claps]")]
        public async Task Clap(int copies)
        {
            string emote = string.Empty;

            if (copies > MAX_CLAPS) copies = MAX_CLAPS;

            for (int i = 0; i < copies; i++)
            {
                emote += "<a:wiiclap:858482992129507348>";
            }
            await Context.Channel.SendMessageAsync(emote);
        }

        [Command("clap", RunMode = RunMode.Async), Summary("Posts wiiclap emote. [Usage] !clap")]
        public async Task Clap()
        {
            await Clap(1);
        }
    }
}
