using Discord.Interactions;
using Feliciabot.net._6._0.helpers;
using WaifuSharp;

namespace Feliciabot.net._6._0.modules
{
    public sealed class RollModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly WaifuClient _waifuClient;

        private readonly string[][] allReponses = [
            [ "As I see it, yes",
            "It is certain!",
            "It is decidedly so!",
            "Most likely!",
            "Outlook good!",
            "Signs point to yes",
            "Without a doubt",
            "Yes",
            "Yes - definitely",
            "You may rely on it"],[
                "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
                ],
                [ "Reply hazy, try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again" ]
    ];

        public RollModule(WaifuClient waifuClient)
        {
            _waifuClient = waifuClient;
        }

        [SlashCommand("8ball", "Answers a question with yes/no/maybe responses", runMode: RunMode.Async)]
        public async Task EightBall(string question)
        {
            int positiveOrNegativeResponse = CommandsHelper.GetRandomNumber(3);
            string[] chosenResponse = allReponses[positiveOrNegativeResponse];
            int randLineIndex = CommandsHelper.GetRandomNumber(chosenResponse.Length - 1);
            await RespondAsync($"Q: {question}\nA: {chosenResponse[randLineIndex]}").ConfigureAwait(false);
        }

        [SlashCommand("roll", "Rolls a 🎲 (default: 6 sided)", runMode: RunMode.Async)]
        public async Task DiceRoll(int sides = 6)
        {
            if (sides <= 0)
            {
                await RespondAsync("Please enter a positive number for the number of sides").ConfigureAwait(false);
                return;
            }

            int randomRoll = CommandsHelper.GetRandomNumber(sides + 1, 1);
            await RespondAsync($"{Context.User.Username} rolled *{randomRoll}*").ConfigureAwait(false);
        }

        [SlashCommand("flip", "Flips a coin", runMode: RunMode.Async)]
        public async Task CoinFlip()
        {
            int headsOrTails = CommandsHelper.GetRandomNumber(2);
            string coinFlipResult = (headsOrTails == 0) ? "Heads" : "Tails";

            await RespondAsync($"{Context.User.Username} got *{coinFlipResult}*").ConfigureAwait(false);
        }

        [SlashCommand("roll-waifu", "Rolls a random waifu from the booru site", runMode: RunMode.Async)]
        public async Task RollWaifu()
        {
            var waifuLink = _waifuClient.GetSfwImage(Endpoints.Sfw.Waifu);
            if (waifuLink is null)
            {
                await RespondAsync("Unable to roll waifu :(");
                return;
            }

            await RespondAsync(waifuLink).ConfigureAwait(false);
        }
    }
}
