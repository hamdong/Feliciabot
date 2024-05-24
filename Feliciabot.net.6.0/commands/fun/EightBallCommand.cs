using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class EightBallCommand : ModuleBase
    {
        private readonly string[] proResponses = {
            "As I see it, yes",
            "It is certain!",
            "It is decidedly so!",
            "Most likely!",
            "Outlook good!",
            "Signs point to yes",
            "Without a doubt",
            "Yes",
            "Yes - definitely",
            "You may rely on it"
        };

        private readonly string[] midResponses = {
            "Reply hazy, try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again"
        };

        private readonly string[] negResponses = {
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        private readonly string[][] allReponses;

        public EightBallCommand()
        {
            allReponses = [proResponses, midResponses, negResponses];
        }

        /// <summary>
        /// Responds to a question with 8ball answers
        /// </summary>
        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("Felicia will answer a question. [Usage]: !8ball")]
        public async Task EightBall([Remainder] string question = "")
        {
            if (string.IsNullOrEmpty(question))
            {
                await Context.Channel.SendMessageAsync("Ask a question!");
                return;
            }

            int positiveOrNegativeResponse = CommandsHelper.GetRandomNumber(3);
            string[] chosenResponse = allReponses[positiveOrNegativeResponse];
            int randLineIndex = CommandsHelper.GetRandomNumber(chosenResponse.Length - 1);
            await Context.Channel.SendMessageAsync(chosenResponse[randLineIndex]);
        }
    }
}
