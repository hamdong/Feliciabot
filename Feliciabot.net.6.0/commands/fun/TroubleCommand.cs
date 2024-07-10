using Discord.Commands;
using Feliciabot.net._6._0.services;

namespace Feliciabot.net._6._0.commands
{
    public class TroubleCommand : ModuleBase<ICommandContext>
    {
        private readonly string[] troubleQuotes = ["WE'VE", "GOT", "TROUBLE!"];
        private readonly MessagingService _msgService;

        public TroubleCommand(MessagingService msgService) => _msgService = msgService;

        [Command("trouble", RunMode = RunMode.Async)]
        [Summary("You have all the information you need")]
        public async Task Trouble()
        {
            foreach (string quote in troubleQuotes)
            {
                await _msgService.SendMessageToChannelById(Context, quote);
                await Task.Delay(1000);
            }
        }
    }
}
