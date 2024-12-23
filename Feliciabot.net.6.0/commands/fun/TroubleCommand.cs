using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands
{
    public class TroubleCommand : ModuleBase
    {
        private readonly string[] troubleQuotes = ["WE'VE", "GOT", "TROUBLE!"];
        private readonly IRandomizerService _randomizerService;
        public TroubleCommand(IRandomizerService randomizerService)
        {
            _randomizerService = randomizerService;
        }

        [Command("trouble", RunMode = RunMode.Async)]
        [Summary("You have all the information you need")]
        public async Task Trouble()
        {
            foreach (string quote in troubleQuotes)
            {
                await Context.Channel.SendMessageAsync(quote);
                await Task.Delay(1000);
            }
        }

        [Command("chairman", RunMode = RunMode.Async)]
        [Summary("Waits some random interval of time before replying 'bana'")]
        public async Task Chairman(bool enableDelay = true)
        {
            int waitInterval = _randomizerService.GetRandom(30000);
            if (enableDelay) await Task.Delay(waitInterval);
            await Context.Channel.SendMessageAsync("bana");
        }
    }
}
