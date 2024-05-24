using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    public class TroubleCommand : ModuleBase
    {
        private const int DELAY_BETWEEN_TROUBLE_MESSAGES = 1000;
        private readonly string[] weveGotTroubleQuotes = ["WE'VE", "GOT", "TROUBLE!"];

        /// <summary>
        /// Posts Felicia's iconic line to the text channel dramatically
        /// </summary>
        /// <returns>Nothing, just posts the lines</returns>
        [Command("trouble", RunMode = RunMode.Async)]
        [Summary("You already know what this does. [Usage] !trouble")]
        public async Task Trouble()
        {
            foreach (string quote in weveGotTroubleQuotes)
            {
                await Context.Channel.SendMessageAsync(quote);
                await Task.Delay(DELAY_BETWEEN_TROUBLE_MESSAGES);
            }
        }
    }
}
