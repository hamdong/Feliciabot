using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class TeaCommand : ModuleBase
    {
        private readonly string[] teaList;
        private const string ALLERGY = "milk";
        private readonly string teasListPath = Environment.CurrentDirectory + @"\data\teas.txt";

        // Shorthand constructor
        public TeaCommand() => teaList = File.ReadAllLines(teasListPath);

        [Command("tea", RunMode = RunMode.Async)]
        [Summary("Calls Felicia to serve a cup of tea to you. [Usage]: !tea")]
        public async Task Tea()
        {
            await ServeTea(Context.User);
        }

        [Command("tea", RunMode = RunMode.Async)]
        [Summary("Calls Felicia to serve a cup of tea to a mentioned user. [Usage]: !tea @user")]
        public async Task Tea(IUser mentionedUser)
        {
            await ServeTea(mentionedUser);
        }

        private async Task ServeTea(IUser user)
        {
            string userToServe = user.Username;
            int randIndex = CommandsHelper.GetRandomNumber(teaList.Length);
            string teaToPost = teaList[randIndex];

            // 1 in 8 odds of spilling
            randIndex = CommandsHelper.GetRandomNumber(8);

            string spilledTeaOnUser = $"*Spilled {teaToPost} on {userToServe} !* :scream:";
            string servedTeaToUser = $"*Served {teaToPost} to {userToServe}.* :tea:";

            string actionTaken = servedTeaToUser;
            string comment = "";

            // Check if we're spilling or serving
            if (randIndex < 1)
            {
                actionTaken = spilledTeaOnUser;
                comment = (userToServe == Context.Client.CurrentUser.Username) ? "Oh no!" : $"Sorry ${userToServe}!";
            }
            else
            {
                if (userToServe == Context.Client.CurrentUser.Username)
                {
                    comment = (teaToPost.Contains(ALLERGY, StringComparison.CurrentCultureIgnoreCase)) ? $"Oh no! I'm allergic to ${ALLERGY}! :scream:" : "Thanks! :smile:";
                }
            }

            await Context.Channel.SendMessageAsync($"{actionTaken} {comment}");
        }
    }
}
