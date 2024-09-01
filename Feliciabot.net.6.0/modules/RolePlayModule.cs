using Discord;
using Discord.Interactions;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services.interfaces;
using WaifuSharp;

namespace Feliciabot.net._6._0.modules
{
    public sealed class RolePlayModule(IWaifuSharpService waifuSharpService) : InteractionModuleBase
    {
        [SlashCommand("bite", "Bite a user", runMode: RunMode.Async)]
        public async Task Bite(IUser user) => await CompileMessage(user, Endpoints.Sfw.Bite, "bit");

        [SlashCommand("blush", "Blush", runMode: RunMode.Async)]
        public async Task Blush() => await CompileMessage(Endpoints.Sfw.Blush, "is blushing");

        [SlashCommand("bully", "Bully a user", runMode: RunMode.Async)]
        public async Task Bully(IUser user) =>
            await CompileMessage(user, Endpoints.Sfw.Bully, "bullied");

        [SlashCommand("cringe", "Cringe", runMode: RunMode.Async)]
        public async Task Cringe() => await CompileMessage(Endpoints.Sfw.Cringe, "is cringing");

        [SlashCommand("cry", "Cry", runMode: RunMode.Async)]
        public async Task Cry() => await CompileMessage(Endpoints.Sfw.Cry, "is crying");

        [SlashCommand("dance", "Dance", runMode: RunMode.Async)]
        public async Task Dance() => await CompileMessage(Endpoints.Sfw.Dance, "is dancing");

        [SlashCommand("happy", "Happy", runMode: RunMode.Async)]
        public async Task Happy() => await CompileMessage(Endpoints.Sfw.Happy, "is happy");

        [SlashCommand("high-five", "High-five a user", runMode: RunMode.Async)]
        public async Task Highfive(IUser user) =>
            await CompileMessage(user, Endpoints.Sfw.Highfive, "high-fived");

        [SlashCommand("hug", "Hug a user", runMode: RunMode.Async)]
        public async Task Hug(IUser user) => await CompileMessage(user, Endpoints.Sfw.Hug, "hugged");

        [SlashCommand("kiss", "Kiss a user", runMode: RunMode.Async)]
        public async Task Kiss(IUser user) => await CompileMessage(user, Endpoints.Sfw.Kiss, "kissed");

        [SlashCommand("nom", "Nom a user", runMode: RunMode.Async)]
        public async Task Nom(IUser user) => await CompileMessage(user, Endpoints.Sfw.Nom, "nommed");

        [SlashCommand("lick", "Lick a user", runMode: RunMode.Async)]
        public async Task Lick(IUser user) => await CompileMessage(user, Endpoints.Sfw.Lick, "licked");

        [SlashCommand("pat", "Pat a user", runMode: RunMode.Async)]
        public async Task Pat(IUser user) => await CompileMessage(user, Endpoints.Sfw.Pat, "patted");

        [SlashCommand("poke", "Poke a user", runMode: RunMode.Async)]
        public async Task Poke(IUser user) => await CompileMessage(user, Endpoints.Sfw.Poke, "poked");

        [SlashCommand("slap", "Slap a user", runMode: RunMode.Async)]
        public async Task Slap(IUser user) => await CompileMessage(user, Endpoints.Sfw.Slap, "slapped");

        [SlashCommand("smug", "Smug", runMode: RunMode.Async)]
        public async Task Smug() => await CompileMessage(Endpoints.Sfw.Smug, "is smug");

        [SlashCommand("wink", "Wink", runMode: RunMode.Async)]
        public async Task Wink() => await CompileMessage(Endpoints.Sfw.Wink, "is winking");

        private async Task CompileMessage(IUser user, Endpoints.Sfw action, string actionOnUser)
        {
            string message = $"{Context.User.GlobalName} {actionOnUser} {user.Mention}";
            string imgURL = waifuSharpService.GetSfwImage(action);
            await BuildEmbedAndRespond(message, imgURL);
        }

        private async Task CompileMessage(Endpoints.Sfw action, string actionOnUser)
        {
            string message = $"{Context.User.GlobalName} {actionOnUser}";
            string imgURL = waifuSharpService.GetSfwImage(action);
            await BuildEmbedAndRespond(message, imgURL);
        }

        private async Task BuildEmbedAndRespond(string message, string imgURL)
        {
            var builder = new EmbedBuilder();
            builder.WithImageUrl(imgURL);
            await RespondAsync(message, embed: builder.Build()).ConfigureAwait(false);
        }
    }
}
