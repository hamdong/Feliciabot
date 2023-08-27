using Discord;
using Discord.Commands;
using WaifuSharp;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to WaifuSharp memes
    /// </summary>
    public class WaifuSharpCommand : ModuleBase
    {
        private readonly WaifuClient _waifuClient;

        /// <summary>
        /// Constructor for WaifuSharp commands
        /// </summary>
        /// <param name="waifuClient">client service to use</param>
        public WaifuSharpCommand(WaifuClient waifuClient)
        {
            _waifuClient = waifuClient;
        }

        /// <summary>
        /// Post a biting gif with the mentioned user
        /// </summary>
        /// <param name="user">User to bite</param>
        [Command("bite", RunMode = RunMode.Async)]
        [Summary("Bite a user. [Usage]: !bite [mentioned user]")]
        public async Task Bite(IUser user) => await PostAction(user, Endpoints.Sfw.Bite, "bit");

        /// <summary>
        /// Post a blushing gif
        /// </summary>
        [Command("blush", RunMode = RunMode.Async)]
        [Summary("Blush. [Usage]: !blush")]
        public async Task Blush() => await PostAction(Endpoints.Sfw.Blush, "is blushing");

        /// <summary>
        /// Post a bullying gif with the mentioned user
        /// </summary>
        /// <param name="user">User to bully</param>
        [Command("bully", RunMode = RunMode.Async)]
        [Summary("Bully a user. [Usage]: !bully [mentioned user]")]
        public async Task Bully(IUser user) => await PostAction(user, Endpoints.Sfw.Bully, "bullied");

        /// <summary>
        /// Post a cringing gif
        /// </summary>
        [Command("cringe", RunMode = RunMode.Async)]
        [Summary("Cringe. [Usage]: !cringe")]
        public async Task Cringe() => await PostAction(Endpoints.Sfw.Cringe, "is cringing");

        /// <summary>
        /// Post a crying gif
        /// </summary>
        [Command("cry", RunMode = RunMode.Async)]
        [Summary("Cry. [Usage]: !cry")]
        public async Task Cry() => await PostAction(Endpoints.Sfw.Cry, "is crying");

        /// <summary>
        /// Post a dancing gif
        /// </summary>
        [Command("dance", RunMode = RunMode.Async)]
        [Summary("Dance. [Usage]: !dance")]
        public async Task Dance() => await PostAction(Endpoints.Sfw.Dance, "is dancing");

        /// <summary>
        /// Post a happy gif
        /// </summary>
        [Command("happy", RunMode = RunMode.Async)]
        [Summary("Happy. [Usage]: !happy")]
        public async Task Happy() => await PostAction(Endpoints.Sfw.Happy, "is happy");

        /// <summary>
        /// Post a highfiving gif with the mentioned user
        /// </summary>
        /// <param name="user">User to highfive</param>
        [Command("highfive", RunMode = RunMode.Async)]
        [Summary("Highfive a user. [Usage]: !highfive [mentioned user]")]
        public async Task Highfive(IUser user) => await PostAction(user, Endpoints.Sfw.Highfive, "highfived");

        /// <summary>
        /// Post a hugging gif with the mentioned user
        /// </summary>
        /// <param name="user">User to hug</param>
        [Command("hug", RunMode = RunMode.Async)]
        [Summary("Hug a user. [Usage]: !hug [mentioned user]")]
        public async Task Hug(IUser user) => await PostAction(user, Endpoints.Sfw.Hug, "hugged");

        /// <summary>
        /// Post a kissing gif with the mentioned user
        /// </summary>
        /// <param name="user">User to kiss</param>
        [Command("kiss", RunMode = RunMode.Async)]
        [Summary("Kiss a user. [Usage]: !kiss [mentioned user]")]
        public async Task Kiss(IUser user) => await PostAction(user, Endpoints.Sfw.Kiss, "kissed");

        /// <summary>
        /// Post a nomming gif with the mentioned user
        /// </summary>
        /// <param name="user">User to nom</param>
        [Command("nom", RunMode = RunMode.Async)]
        [Summary("Nom a user. [Usage]: !nom [mentioned user]")]
        public async Task Nom(IUser user) => await PostAction(user, Endpoints.Sfw.Nom, "nommed");

        /// <summary>
        /// Post a licking gif with the mentioned user
        /// </summary>
        /// <param name="user">User to lick</param>
        [Command("lick", RunMode = RunMode.Async)]
        [Summary("Lick a user. [Usage]: !lick [mentioned user]")]
        public async Task Lick(IUser user) => await PostAction(user, Endpoints.Sfw.Lick, "licked");

        /// <summary>
        /// Post a patting gif with the mentioned user
        /// </summary>
        /// <param name="user">User to pat</param>
        [Command("pat", RunMode = RunMode.Async)]
        [Summary("Pat a user. [Usage]: !pat [mentioned user]")]
        public async Task Pat(IUser user) => await PostAction(user, Endpoints.Sfw.Pat, "patted");

        /// <summary>
        /// Post a poking gif with the mentioned user
        /// </summary>
        /// <param name="user">User to poke</param>
        [Command("poke", RunMode = RunMode.Async)]
        [Summary("Poke a user. [Usage]: !poke [mentioned user]")]
        public async Task Poke(IUser user) => await PostAction(user, Endpoints.Sfw.Poke, "poked");

        /// <summary>
        /// Post a slapping gif with the mentioned user
        /// </summary>
        /// <param name="user">User to slap</param>
        [Command("slap", RunMode = RunMode.Async)]
        [Summary("Slap a user. [Usage]: !slap [mentioned user]")]
        public async Task Slap(IUser user) => await PostAction(user, Endpoints.Sfw.Slap, "slapped");

        /// <summary>
        /// Remind user to mention a user to use these commands
        /// </summary>
        [Command("slap", RunMode = RunMode.Async)]
        [Summary("Remind user to mention a user to use these commands.")]
        [Alias("bite", "bully", "highfive", "hug", "nom", "lick", "pat", "poke", "kiss")]
        public async Task ActionWithNoMention()
        {
            await Context.Channel.SendMessageAsync("You need to mention a user to use this command! :open_mouth:");
        }

        /// <summary>
        /// Post a smugging gif
        /// </summary>
        [Command("smug", RunMode = RunMode.Async)]
        [Summary("Smug. [Usage]: !smug")]
        public async Task Smug()
        {
            await PostAction(Endpoints.Sfw.Smug, "is smug");
        }

        /// <summary>
        /// Post a winking gif
        /// </summary>
        [Command("wink", RunMode = RunMode.Async)]
        [Summary("Wink. [Usage]: !wink")]
        public async Task Wink()
        {
            await PostAction(Endpoints.Sfw.Wink, "is winking");
        }

        /// <summary>
        /// Post the WaifuSharp action with user context and gif
        /// </summary>
        /// <param name="user">User to post action for</param>
        /// <param name="action">Waifusharp Endpoint Enum action</param>
        /// <param name="actionPastTense">Action in past tense, for the message title</param>
        private async Task PostAction(IUser user, Endpoints.Sfw action, string actionPastTense)
        {
            string text = Context.User.Username + " " + actionPastTense + " " + user.Username;
            await PostTextWithAction(text, action);
        }

        /// <summary>
        /// Create the text before posting the WaifuSharp action with gif
        /// </summary>
        /// <param name="action">Waifusharp Endpoint Enum action</param>
        /// <param name="actionPastTense">Action in past tense, for the message title</param>
        private async Task PostAction(Endpoints.Sfw action, string actionPastTense)
        {
            string text = Context.User.Username + " " + actionPastTense;
            await PostTextWithAction(text, action);
        }

        /// <summary>
        /// Post the WaifuSharp action with text and gif
        /// </summary>
        /// <param name="text">Text to go with post</param>
        /// <param name="action">Waifusharp Endpoint Enum action</param>
        private async Task PostTextWithAction(string text, Endpoints.Sfw action)
        {
            string imgURL = _waifuClient.GetSfwImage(action);

            var builder = new EmbedBuilder { Title = text };
            builder.WithImageUrl(imgURL);

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
