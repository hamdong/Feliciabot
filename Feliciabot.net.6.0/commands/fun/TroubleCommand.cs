using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    public class TroubleCommand : ModuleBase
    {
        private readonly string[] troubleQuotes = ["WE'VE", "GOT", "TROUBLE!"];

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
    }
}
