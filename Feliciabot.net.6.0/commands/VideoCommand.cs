using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to posting videos
    /// </summary>
    public class VideoCommand : ModuleBase
    {
        private readonly string INDEED_VIDEO_LINK = "https://www.youtube.com/watch?v=T5S4r2p9x34";
        private readonly string INDEED_VIDEO_LINK_ALT = "https://www.youtube.com/watch?v=3fVo_Zy42FU";
        private readonly List<string> BOCCHI_VIDEOS = new() {
            "https://www.youtube.com/watch?v=Q04Va_1JI04",
            "https://www.youtube.com/shorts/hbFJSrx3sg0",
            "https://www.youtube.com/watch?v=tjAsZ6bHCZE"
        };
        private readonly List<string> YIPPEE_VIDEOS = new()
        {
            Environment.CurrentDirectory + @"\videos\yippee.mov",
            Environment.CurrentDirectory + @"\videos\brigus.mp4"
        };
        private readonly string GG_VIDEO_LINK = "https://www.youtube.com/watch?v=9nXYsmTv3Gg";
        private readonly string GANBARE_VIDEO_LINK = "https://www.youtube.com/watch?v=YoHq6DrWLSI";

        /// <summary>
        /// Post GG
        /// </summary>
        /// <returns></returns>
        [Command("gg", RunMode = RunMode.Async)]
        [Summary("Posts 'GG' video. [Usage]: !gg")]
        public async Task GG()
        {
            await Context.Channel.SendMessageAsync(GG_VIDEO_LINK);
        }

        /// <summary>
        /// Post Ganbare
        /// </summary>
        /// <returns></returns>
        [Command("ganbare", RunMode = RunMode.Async)]
        [Summary("Posts 'Ganbare' video. [Usage]: !gg")]
        public async Task Ganbare()
        {
            await Context.Channel.SendMessageAsync(GANBARE_VIDEO_LINK);
        }

        /// <summary>
        /// Post indeed
        /// </summary>
        /// <returns></returns>
        [Command("indeed", RunMode = RunMode.Async)]
        [Summary("Posts 'Indeed' video. [Usage]: !indeed")]
        public async Task Indeed()
        {
            // 1 in 6 odds of posting the alternative video
            int randIndex = CommandsHelper.GetRandomNumber(5);
            string videoToPost = randIndex < 1 ? INDEED_VIDEO_LINK_ALT : INDEED_VIDEO_LINK;

            await Context.Channel.SendMessageAsync(videoToPost);
        }

        /// <summary>
        /// Post Malos hi
        /// </summary>
        /// <returns></returns>
        [Alias("hello", "owain")]
        [Command("hi", RunMode = RunMode.Async)]
        [Summary("Posts 'Hi' video. [Usage]: !hi, !hello, !owain")]
        public async Task Hi()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\video0.mov");
        }

        /// <summary>
        /// Post random wahaha
        /// </summary>
        /// <returns></returns>
        [Command("wahaha", RunMode = RunMode.Async)]
        [Summary("Posts random wahaha video. [Usage]: !wahaha")]
        public async Task Wahaha()
        {
            await Context.Channel.SendMessageAsync(BOCCHI_VIDEOS[CommandsHelper.GetRandomNumber(BOCCHI_VIDEOS.Count)]);
        }

        /// <summary>
        /// Post Sena Yippee
        /// </summary>
        /// <returns></returns>
        [Alias("sena", "yipee")]
        [Command("yippee", RunMode = RunMode.Async)]
        [Summary("Posts 'Yippee!' video. [Usage]: !yippee")]
        public async Task Yippee()
        {
            await Context.Channel.SendFileAsync(YIPPEE_VIDEOS[CommandsHelper.GetRandomNumber(YIPPEE_VIDEOS.Count)]);
        }
    }
}
