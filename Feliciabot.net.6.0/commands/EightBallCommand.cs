using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class EightBallCommand : ModuleBase
    {
        private readonly string[] answers_positive = {
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

        private readonly string[] answers_maybe = {
            "Reply hazy, try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again"
        };

        private readonly string[] answers_negative = {
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        /// <summary>
        /// Responds to a question with 8ball answers
        /// </summary>
        /// <returns>Nothing, posts the answer in the channel</returns>
        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("Felicia will answer a question. [Usage]: !8ball")]
        public async Task EightBall([Remainder] string question = "")
        {
            string answer;
            if (question.Trim() == "")
            {
                answer = "Ask a question!";
            }
            else
            {
                int positiveOrNegativeResponse = CommandsHelper.GetRandomNumber(2);

                int randLineIndex;
                switch (positiveOrNegativeResponse)
                {
                    case 0:
                        randLineIndex = CommandsHelper.GetRandomNumber(answers_positive.Length - 1);
                        answer = answers_positive[randLineIndex];
                        break;
                    case 1:
                        randLineIndex = CommandsHelper.GetRandomNumber(answers_maybe.Length - 1);
                        answer = answers_maybe[randLineIndex];
                        break;
                    default:
                        randLineIndex = CommandsHelper.GetRandomNumber(answers_negative.Length - 1);
                        answer = answers_negative[randLineIndex];
                        break;
                }
            }

            await Context.Channel.SendMessageAsync(answer);
        }
    }
}
