using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands.fun
{
    public class TeaCommand : ModuleBase
    {
        private readonly string[] teaList = Responses.TeaResponses;
        private readonly IRandomizerService _randomizerService;

        public TeaCommand(IRandomizerService randomizerService) =>
            _randomizerService = randomizerService;

        [Command("tea", RunMode = RunMode.Async)]
        [Summary("Felicia will serve a cup of tea to you. [Usage]: !tea")]
        public async Task Tea()
        {
            await ServeTea(Context.User);
        }

        [Command("tea", RunMode = RunMode.Async)]
        [Summary("Felicia will serve a cup of tea to a mentioned user. [Usage]: !tea @user")]
        public async Task Tea(IUser mentionedUser)
        {
            await ServeTea(mentionedUser);
        }

        private async Task ServeTea(IUser user)
        {
            string userToServe = user.GlobalName;
            int randIndex = _randomizerService.GetRandom(teaList.Length);
            string selectedTea = teaList[randIndex];
            randIndex = _randomizerService.GetRandom(8); // 1 in 8 odds of spilling

            string response =
                randIndex < 1
                    ? $"*Spilled {selectedTea} tea on {userToServe}!* :scream:"
                    : $"*Served {selectedTea} tea to {userToServe}.* :tea:";

            string comment = "";

            if (randIndex < 1)
            {
                comment =
                    userToServe == Context.Client.CurrentUser.GlobalName
                        ? "Oh no!"
                        : $"Sorry {userToServe}!";
            }
            else
            {
                if (userToServe == Context.Client.CurrentUser.GlobalName)
                {
                    comment = selectedTea.Contains(
                        "milk",
                        StringComparison.CurrentCultureIgnoreCase
                    )
                        ? $"Oh no! I'm allergic to milk! :scream:"
                        : "Thanks! :smile:";
                }
            }

            await Context.Channel.SendMessageAsync($"{response} {comment}");
        }
    }
}
