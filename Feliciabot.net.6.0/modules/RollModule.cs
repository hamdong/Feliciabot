using Discord.Interactions;
using Feliciabot.net._6._0.helpers;
using WaifuSharp;

namespace Feliciabot.net._6._0.modules
{
    public sealed class RollModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly WaifuClient _waifuClient;
        public RollModule(WaifuClient waifuClient)
        {
            _waifuClient = waifuClient;
        }

        [SlashCommand("roll", "Rolls a 🎲 (default: 6 sided)", runMode: RunMode.Async)]
        public async Task DiceRoll(int sides = 6)
        {
            if(sides <= 0)
            {
                await RespondAsync("Please enter a positive number for the number of sides");
                return;
            }

            int randomRoll = CommandsHelper.GetRandomNumber(sides + 1, 1);
            await RespondAsync($"{Context.User.Username} rolled *{randomRoll}*");
        }

        [SlashCommand("flip", "Flips a coin", runMode: RunMode.Async)]
        public async Task CoinFlip()
        {
            int headsOrTails = CommandsHelper.GetRandomNumber(2);
            string coinFlipResult = (headsOrTails == 0) ? "Heads" : "Tails";

            await RespondAsync($"{Context.User.Username} got *{coinFlipResult}*");
        }

        [SlashCommand("roll-waifu", "Rolls a random waifu from the booru site", runMode: RunMode.Async)]
        public async Task RollWaifu()
        {
            var waifuLink = _waifuClient.GetSfwImage(Endpoints.Sfw.Waifu);
            if(waifuLink is null)
            {
                await RespondAsync("Unable to roll waifu :(");
                return;
            }

            await RespondAsync(waifuLink);
        }
    }
}
