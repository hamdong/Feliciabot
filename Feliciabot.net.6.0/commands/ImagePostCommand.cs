using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to posting random images
    /// </summary>
    public class ImagePostCommand : ModuleBase
    {
        private readonly string AWESOME_IMAGE_PATH = Environment.CurrentDirectory + @"\img\awesome.jpg";
        private readonly string BANJO_IMAGE_PATH = Environment.CurrentDirectory + @"\img\banjo.jpg";
        private readonly string BONK_IMAGE_PATH = Environment.CurrentDirectory + @"\img\bonk.jpg";
        private readonly string BUGGIN_IMAGE_PATH = Environment.CurrentDirectory + @"\img\buggin.png";
        private readonly string CRUEL_IMAGE_PATH = Environment.CurrentDirectory + @"\img\cruel.png";
        private readonly string FIREEMBLEM_IMAGE_PATH = Environment.CurrentDirectory + @"\img\fireemblem.png";
        private readonly string HOME_IMAGE_PATH = Environment.CurrentDirectory + @"\img\home.jpg";
        private readonly string PATHETIC_IMAGE_PATH = Environment.CurrentDirectory + @"\img\pathetic.png";
        private readonly string POG_IMAGE_PATH = Environment.CurrentDirectory + @"\img\pog.jpg";
        private readonly string SHOCK_IMAGE_PATH = Environment.CurrentDirectory + @"\img\shock.jpg";
        private readonly string STARE_IMAGE_PATH = Environment.CurrentDirectory + @"\img\stare.gif";
        private readonly string STUPID_IMAGE_PATH = Environment.CurrentDirectory + @"\img\stupid.png";
        private readonly string XENOBLADE_IMAGE_PATH = Environment.CurrentDirectory + @"\img\xenoblade.png";

        /// <summary>
        /// Post awesome
        /// </summary>
        /// <returns></returns>
        [Command("awesome", RunMode = RunMode.Async)]
        [Summary("Posts 'AWESOME' meme. [Usage]: !awesome")]
        public async Task Awesome()
        {
            await PostToChannel(AWESOME_IMAGE_PATH);
        }

        /// <summary>
        /// Post banjo
        /// </summary>
        /// <returns></returns>
        [Command("banjo", RunMode = RunMode.Async)]
        [Summary("Posts 'Banjo' meme. [Usage]: !banjo")]
        public async Task Banjo()
        {
            await PostToChannel(BANJO_IMAGE_PATH);
        }

        /// <summary>
        /// Post bonk
        /// </summary>
        /// <returns></returns>
        [Command("bonk", RunMode = RunMode.Async)]
        [Summary("Posts 'Bonk' meme. [Usage]: !bonk")]
        public async Task Bonk()
        {
            await PostToChannel(BONK_IMAGE_PATH);
        }

        /// <summary>
        /// Post buggin
        /// </summary>
        /// <returns></returns>
        [Command("buggin", RunMode = RunMode.Async)]
        [Summary("Posts 'Something bugging you?' meme. [Usage]: !buggin")]
        public async Task Buggin()
        {
            await PostToChannel(BUGGIN_IMAGE_PATH);
        }

        /// <summary>
        /// Post cruel
        /// </summary>
        /// <returns></returns>
        [Command("cruel", RunMode = RunMode.Async)]
        [Summary("Posts 'It's a tentacruel world' meme. [Usage]: !cruel")]
        public async Task Cruel()
        {
            await PostToChannel(CRUEL_IMAGE_PATH);
        }

        /// <summary>
        /// Post fire emblem
        /// </summary>
        /// <returns></returns>
        [Command("fireemblem", RunMode = RunMode.Async)]
        [Summary("Posts 'war crimes' meme. [Usage]: !fireemblem")]
        public async Task FireEmblem()
        {
            await PostToChannel(FIREEMBLEM_IMAGE_PATH);
        }

        /// <summary>
        /// Post I wanna go home
        /// </summary>
        /// <returns></returns>
        [Command("home", RunMode = RunMode.Async)]
        [Summary("Posts 'I wanna go home' meme. [Usage]: !home")]
        public async Task Home()
        {
            await PostToChannel(HOME_IMAGE_PATH);
        }

        /// <summary>
        /// Post pathetic
        /// </summary>
        /// <returns></returns>
        [Command("pathetic", RunMode = RunMode.Async)]
        [Summary("Posts 'pathetic' meme. [Usage]: !pathetic")]
        public async Task Pathetic()
        {
            await PostToChannel(PATHETIC_IMAGE_PATH);
        }

        /// <summary>
        /// Post pog
        /// </summary>
        /// <returns></returns>
        [Command("pog", RunMode = RunMode.Async)]
        [Summary("Posts 'pog' meme. [Usage]: !pog")]
        public async Task Pog()
        {
            await PostToChannel(POG_IMAGE_PATH);
        }

        /// <summary>
        /// Post shock
        /// </summary>
        /// <returns></returns>
        [Command("shock", RunMode = RunMode.Async)]
        [Summary("Posts 'shock' meme. [Usage]: !shock")]
        public async Task Shock()
        {
            await PostToChannel(SHOCK_IMAGE_PATH);
        }

        /// <summary>
        /// Post stare
        /// </summary>
        /// <returns></returns>
        [Command("stare", RunMode = RunMode.Async)]
        [Summary("Posts 'stare' meme. [Usage]: !stare")]
        public async Task Stare()
        {
            await PostToChannel(STARE_IMAGE_PATH);
        }

        /// <summary>
        /// Post stupid
        /// </summary>
        /// <returns></returns>
        [Command("stupid", RunMode = RunMode.Async)]
        [Summary("Posts 'I may be stupid' meme. [Usage]: !stupid")]
        public async Task Stupid()
        {
            await PostToChannel(STUPID_IMAGE_PATH);
        }

        /// <summary>
        /// Post Xenoblade
        /// </summary>
        /// <returns></returns>
        [Command("xenoblade", RunMode = RunMode.Async)]
        [Summary("Posts 'Holy s* it's the xenoblade' meme. [Usage]: !xenoblade")]
        public async Task Xenoblade()
        {
            await PostToChannel(XENOBLADE_IMAGE_PATH);
        }

        /// <summary>
        /// Post specified image path
        /// </summary>
        /// <param name="filePathToPost">Path to file to post</param>
        /// <returns>Nothing, posts the image in the channel</returns>
        private async Task PostToChannel(string filePathToPost)
        {
            await Context.Channel.SendFileAsync(filePathToPost, "");
        }
    }
}
