﻿using Discord.Commands;
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

        private readonly List<string> BOCCHI_VIDEOS = new()
        {
            "https://www.youtube.com/watch?v=Q04Va_1JI04",
            "https://www.youtube.com/shorts/hbFJSrx3sg0",
            "https://www.youtube.com/watch?v=tjAsZ6bHCZE"
        };

        private readonly string GG_VIDEO_LINK = "https://www.youtube.com/watch?v=9nXYsmTv3Gg";
        private readonly string GANBARE_VIDEO_LINK = "https://www.youtube.com/watch?v=YoHq6DrWLSI";
        private readonly string YIPPEE_FOLDER_PATH = Path.Combine(Environment.CurrentDirectory, "videos", "yippee");

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
        /// Post huh video
        /// </summary>
        /// <returns></returns>
        [Command("huh", RunMode = RunMode.Async)]
        [Summary("Posts huh video. [Usage]: !huh")]
        public async Task Huh()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\huh.mp4");
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
        /// Post shutup video
        /// </summary>
        /// <returns></returns>
        [Command("shutup", RunMode = RunMode.Async)]
        [Summary("Posts shutup video. [Usage]: !shutup")]
        public async Task Shutup()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\shutup.mp4");
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
        /// Post what video
        /// </summary>
        /// <returns></returns>
        [Alias("whatareyoutalkingabout", "whatsgoinonhere")]
        [Command("what", RunMode = RunMode.Async)]
        [Summary("Posts what are you talking about video. [Usage]: !what")]
        public async Task What()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\what.mov");
        }

        /// <summary>
        /// Post Yippee
        /// </summary>
        /// <returns></returns>
        [Alias("sena", "yipee")]
        [Command("yippee", RunMode = RunMode.Async)]
        [Summary("Posts 'Yippee!' video. [Usage]: !yippee")]
        public async Task Yippee()
        {
            var videoFiles = Directory.GetFiles(YIPPEE_FOLDER_PATH, "*.*")
                .Where(s => Path.GetExtension(s).Equals(".mov", StringComparison.OrdinalIgnoreCase) ||
                Path.GetExtension(s).Equals(".mp4", StringComparison.OrdinalIgnoreCase));

            if (!videoFiles.Any())
            {
                await Context.Channel.SendMessageAsync("No yippee videos found...");
            }

            var yippeeVideo = videoFiles.ElementAt(CommandsHelper.GetRandomNumber(videoFiles.Count()));
            await Context.Channel.SendFileAsync(yippeeVideo);
        }
    }
}
