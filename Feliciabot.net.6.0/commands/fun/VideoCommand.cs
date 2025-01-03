﻿using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands.fun
{
    public class VideoCommand : ModuleBase
    {
        private readonly string INDEED_VIDEO_LINK = "https://www.youtube.com/watch?v=T5S4r2p9x34";
        private readonly string INDEED_VIDEO_LINK_ALT = "https://www.youtube.com/watch?v=3fVo_Zy42FU";

        private readonly List<string> BOCCHI_VIDEOS =
        [
            "https://www.youtube.com/watch?v=Q04Va_1JI04",
            "https://www.youtube.com/shorts/hbFJSrx3sg0",
            "https://www.youtube.com/watch?v=tjAsZ6bHCZE"
        ];

        private readonly string GG_VIDEO_LINK = "https://www.youtube.com/watch?v=9nXYsmTv3Gg";
        private readonly string GANBARE_VIDEO_LINK = "https://www.youtube.com/watch?v=YoHq6DrWLSI";
        private readonly string YIPPEE_FOLDER_PATH = Path.Combine(Environment.CurrentDirectory, "videos", "yippee");

        private readonly IRandomizerService _randomizerService;

        public VideoCommand(IRandomizerService randomizerService) =>
            _randomizerService = randomizerService;

        [Alias("alfie")]
        [Command("alfred", RunMode = RunMode.Async)]
        [Summary("Posts 'Alfred' video")]
        public async Task Alfred()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\alfred.mov");
        }

        [Alias("arover, itsaruover, itsover")]
        [Command("aruover", RunMode = RunMode.Async)]
        [Summary("Posts 'Aruover' video")]
        public async Task Aruover()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\aruover.mp4");
        }

        [Command("gg", RunMode = RunMode.Async)]
        [Summary("Posts 'GG' video")]
        public async Task GG()
        {
            await Context.Channel.SendMessageAsync(GG_VIDEO_LINK);
        }

        [Command("ganbare", RunMode = RunMode.Async)]
        [Summary("Posts 'Ganbare' video")]
        public async Task Ganbare()
        {
            await Context.Channel.SendMessageAsync(GANBARE_VIDEO_LINK);
        }

        [Command("huh", RunMode = RunMode.Async)]
        [Summary("Posts huh video")]
        public async Task Huh()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\huh.mp4");
        }

        [Alias("hello", "owain")]
        [Command("hi", RunMode = RunMode.Async)]
        [Summary("Posts 'Hi' video")]
        public async Task Hi()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\video0.mov");
        }

        [Command("indeed", RunMode = RunMode.Async)]
        [Summary("Posts 'Indeed' video")]
        public async Task Indeed()
        {
            // 1 in 6 odds of posting the alternative video
            int randIndex = _randomizerService.GetRandom(5);
            string videoToPost = randIndex < 1 ? INDEED_VIDEO_LINK_ALT : INDEED_VIDEO_LINK;

            await Context.Channel.SendMessageAsync(videoToPost);
        }

        [Command("shutup", RunMode = RunMode.Async)]
        [Summary("Posts shutup video")]
        public async Task Shutup()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\shutup.mp4");
        }

        [Command("wahaha", RunMode = RunMode.Async)]
        [Summary("Posts random wahaha video")]
        public async Task Wahaha()
        {
            await Context.Channel.SendMessageAsync(BOCCHI_VIDEOS[_randomizerService.GetRandom(BOCCHI_VIDEOS.Count)]);
        }

        [Alias("whatareyoutalkingabout", "whatsgoinonhere")]
        [Command("what", RunMode = RunMode.Async)]
        [Summary("Posts what are you talking about video")]
        public async Task What()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\what.mov");
        }

        [Alias("sena", "yipee")]
        [Command("yippee", RunMode = RunMode.Async)]
        [Summary("Posts 'Yippee!' video")]
        public async Task Yippee()
        {
            var videoFiles = Directory.GetFiles(YIPPEE_FOLDER_PATH, "*.*")
                .Where(s => Path.GetExtension(s).Equals(".mov", StringComparison.OrdinalIgnoreCase) ||
                Path.GetExtension(s).Equals(".mp4", StringComparison.OrdinalIgnoreCase));

            if (!videoFiles.Any())
            {
                await Context.Channel.SendMessageAsync("No yippee videos found...");
            }

            var yippeeVideo = videoFiles.ElementAt(_randomizerService.GetRandom(videoFiles.Count()));
            await Context.Channel.SendFileAsync(yippeeVideo);
        }
    }
}
