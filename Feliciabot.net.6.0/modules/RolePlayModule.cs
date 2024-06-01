using Discord;
using Discord.Interactions;
using WaifuSharp;

namespace Feliciabot.net._6._0.modules
{
    public sealed class RolePlayModule(WaifuClient waifuClient) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("bite", "Bite a user", runMode: RunMode.Async)]
        public async Task Bite(IUser user) => await PostAction(user, Endpoints.Sfw.Bite, "bit");

        [SlashCommand("blush", "Blush", runMode: RunMode.Async)]
        public async Task Blush() => await PostAction(Endpoints.Sfw.Blush, "is blushing");

        [SlashCommand("bully", "Bully a user", runMode: RunMode.Async)]
        public async Task Bully(IUser user) => await PostAction(user, Endpoints.Sfw.Bully, "bullied");

        [SlashCommand("cringe", "Cringe", runMode: RunMode.Async)]
        public async Task Cringe() => await PostAction(Endpoints.Sfw.Cringe, "is cringing");

        [SlashCommand("cry", "Cry", runMode: RunMode.Async)]
        public async Task Cry() => await PostAction(Endpoints.Sfw.Cry, "is crying");

        [SlashCommand("dance", "Dance", runMode: RunMode.Async)]
        public async Task Dance() => await PostAction(Endpoints.Sfw.Dance, "is dancing");

        [SlashCommand("happy", "Happy", runMode: RunMode.Async)]
        public async Task Happy() => await PostAction(Endpoints.Sfw.Happy, "is happy");

        [SlashCommand("high-five", "High-five a user", runMode: RunMode.Async)]
        public async Task Highfive(IUser user) => await PostAction(user, Endpoints.Sfw.Highfive, "high-fived");

        [SlashCommand("hug", "Hug a user", runMode: RunMode.Async)]
        public async Task Hug(IUser user) => await PostAction(user, Endpoints.Sfw.Hug, "hugged");

        [SlashCommand("kiss", "Kiss a user", runMode: RunMode.Async)]
        public async Task Kiss(IUser user) => await PostAction(user, Endpoints.Sfw.Kiss, "kissed");

        [SlashCommand("nom", "Nom a user", runMode: RunMode.Async)]
        public async Task Nom(IUser user) => await PostAction(user, Endpoints.Sfw.Nom, "nommed");

        [SlashCommand("lick", "Lick a user", runMode: RunMode.Async)]
        public async Task Lick(IUser user) => await PostAction(user, Endpoints.Sfw.Lick, "licked");

        [SlashCommand("pat", "Pat a user", runMode: RunMode.Async)]
        public async Task Pat(IUser user) => await PostAction(user, Endpoints.Sfw.Pat, "patted");

        [SlashCommand("poke", "Poke a user", runMode: RunMode.Async)]
        public async Task Poke(IUser user) => await PostAction(user, Endpoints.Sfw.Poke, "poked");

        [SlashCommand("slap", "Slap a user", runMode: RunMode.Async)]
        public async Task Slap(IUser user) => await PostAction(user, Endpoints.Sfw.Slap, "slapped");

        [SlashCommand("smug", "Smug", runMode: RunMode.Async)]
        public async Task Smug() => await PostAction(Endpoints.Sfw.Smug, "is smug");

        [SlashCommand("wink", "Wink", runMode: RunMode.Async)]
        public async Task Wink() => await PostAction(Endpoints.Sfw.Wink, "is winking");

        private async Task PostAction(IUser user, Endpoints.Sfw action, string actionPastTense)
        {
            string text = $"{Context.User.GlobalName} {actionPastTense} {user.Mention}";
            await PostTextWithAction(text, action);
        }

        private async Task PostAction(Endpoints.Sfw action, string actionPastTense)
        {
            string text = $"{Context.User.GlobalName} {actionPastTense}";
            await PostTextWithAction(text, action);
        }

        private async Task PostTextWithAction(string text, Endpoints.Sfw action)
        {
            var builder = new EmbedBuilder();
            string imgURL = waifuClient.GetSfwImage(action);
            builder.WithImageUrl(imgURL);

            await RespondAsync(text, embed: builder.Build()).ConfigureAwait(false);
        }
    }
}
