using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands
{
    public class TroubleCommand : ModuleBase<ICommandContext>
    {
        private readonly string[] troubleQuotes = ["WE'VE", "GOT", "TROUBLE!"];
        private readonly IMessagingService _msgService;

        public TroubleCommand(IMessagingService msgService) => _msgService = msgService;

        [Command("trouble", RunMode = RunMode.Async)]
        [Summary("You have all the information you need")]
        public async Task Trouble()
        {
            foreach (string quote in troubleQuotes)
            {
                await _msgService.SendMessageToContextAsync(Context, quote);
                await Task.Delay(1000);
            }
        }

        [Command("chairman", RunMode = RunMode.Async)]
        [Summary("Waits some random interval of time before replying 'bana'")]
        public async Task Chairman()
        {
            int waitInterval = CommandsHelper.GetRandomNumber(30000);
            await Task.Delay(waitInterval);
            await _msgService.SendMessageToContextAsync(Context, "bana");
        }
    }
}
