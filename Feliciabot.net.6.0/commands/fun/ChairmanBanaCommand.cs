using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class ChairmanBanaCommand : ModuleBase
    {
        /// <summary>
        /// Posts bana after some delay as a response to chairman
        /// </summary>
        [Command("chairman", RunMode = RunMode.Async)]
        [Summary("Waits some random interval of time before replying bana. [Usage]: !chairman")]
        public async Task Chairman()
        {
            int waitInterval = CommandsHelper.GetRandomNumber(60000);
            await Task.Delay(waitInterval);
            await Context.Channel.SendMessageAsync("bana");
        }
    }
}
