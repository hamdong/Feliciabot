using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands.fun
{
    public class VideoCommand : ModuleBase
    {
        private readonly string INDEED_VIDEO_LINK = "https://www.youtube.com/watch?v=T5S4r2p9x34";
        private readonly string INDEED_VIDEO_LINK_ALT =
            "https://www.youtube.com/watch?v=3fVo_Zy42FU";

        private readonly List<string> BOCCHI_VIDEOS = new List<string>
        {
            "https://www.youtube.com/watch?v=Q04Va_1JI04",
            "https://www.youtube.com/shorts/hbFJSrx3sg0",
            "https://www.youtube.com/watch?v=tjAsZ6bHCZE",
        };

        private readonly string GG_VIDEO_LINK = "https://www.youtube.com/watch?v=9nXYsmTv3Gg";
        private readonly string GANBARE_VIDEO_LINK = "https://www.youtube.com/watch?v=YoHq6DrWLSI";

        private readonly string BASE_PATH = Environment.CurrentDirectory;
        private readonly string YIPPEE_FOLDER_PATH;

        private readonly IRandomizerService _randomizerService;

        public VideoCommand(IRandomizerService randomizerService)
        {
            _randomizerService = randomizerService;
            YIPPEE_FOLDER_PATH = Path.Combine(BASE_PATH, "videos", "yippee");
        }

        [Alias("alfie")]
        [Command("alfred", RunMode = RunMode.Async)]
        [Summary("Posts 'Alfred' video")]
        public async Task Alfred()
        {
            var filePath = Path.Combine(BASE_PATH, "videos", "alfred.mov");
            await Context.Channel.SendFileAsync(filePath);
        }

        [Alias("arover", "itsaruover", "itsover")]
        [Command("aruover", RunMode = RunMode.Async)]
        [Summary("Posts 'Aruover' video")]
        public async Task Aruover()
        {
            var filePath = Path.Combine(BASE_PATH, "videos", "aruover.mp4");
            await Context.Channel.SendFileAsync(filePath);
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
            var filePath = Path.Combine(BASE_PATH, "videos", "huh.mp4");
            await Context.Channel.SendFileAsync(filePath);
        }

        [Alias("hello", "owain")]
        [Command("hi", RunMode = RunMode.Async)]
        [Summary("Posts 'Hi' video")]
        public async Task Hi()
        {
            var filePath = Path.Combine(BASE_PATH, "videos", "video0.mov");
            await Context.Channel.SendFileAsync(filePath);
        }

        [Command("indeed", RunMode = RunMode.Async)]
        [Summary("Posts 'Indeed' video")]
        public async Task Indeed()
        {
            // 1 in 6 odds of posting the alternative video
            int randIndex = _randomizerService.GetRandom(6);
            string videoToPost = randIndex < 1 ? INDEED_VIDEO_LINK_ALT : INDEED_VIDEO_LINK;

            await Context.Channel.SendMessageAsync(videoToPost);
        }

        [Command("shutup", RunMode = RunMode.Async)]
        [Summary("Posts shutup video")]
        public async Task Shutup()
        {
            var filePath = Path.Combine(BASE_PATH, "videos", "shutup.mp4");
            await Context.Channel.SendFileAsync(filePath);
        }

        [Command("wahaha", RunMode = RunMode.Async)]
        [Summary("Posts random wahaha video")]
        public async Task Wahaha()
        {
            var randomIndex = _randomizerService.GetRandom(BOCCHI_VIDEOS.Count);
            await Context.Channel.SendMessageAsync(BOCCHI_VIDEOS[randomIndex]);
        }

        [Alias("whatareyoutalkingabout", "whatsgoinonhere")]
        [Command("what", RunMode = RunMode.Async)]
        [Summary("Posts what are you talking about video")]
        public async Task What()
        {
            var filePath = Path.Combine(BASE_PATH, "videos", "what.mov");
            await Context.Channel.SendFileAsync(filePath);
        }

        [Alias("sena", "yipee")]
        [Command("yippee", RunMode = RunMode.Async)]
        [Summary("Posts 'Yippee!' video")]
        public async Task Yippee()
        {
            var videoFiles = Directory
                .GetFiles(YIPPEE_FOLDER_PATH, "*.*")
                .Where(s =>
                    Path.GetExtension(s).Equals(".mov", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(s).Equals(".mp4", StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (!videoFiles.Any())
            {
                await Context.Channel.SendMessageAsync("No yippee videos found...");
                return;
            }

            var randomIndex = _randomizerService.GetRandom(videoFiles.Count);
            var yippeeVideo = videoFiles[randomIndex];
            await Context.Channel.SendFileAsync(yippeeVideo);
        }
    }
}
