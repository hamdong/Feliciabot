using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.commands.fun
{
    public class ImageCommand : ModuleBase
    {
        private readonly string env = Path.Combine(Environment.CurrentDirectory, "img");
        private readonly IRandomizerService _randomizerService;

        public ImageCommand(IRandomizerService randomizerService)
        {
            _randomizerService = randomizerService;
        }

        [Command("awesome", RunMode = RunMode.Async)]
        [Summary("Posts 'AWESOME' meme")]
        public async Task Awesome() => await PostToChannel(Path.Combine(env, "awesome.jpg"));

        [Command("banjo", RunMode = RunMode.Async)]
        [Summary("Posts 'Banjo' meme")]
        public async Task Banjo() => await PostToChannel(Path.Combine(env, "banjo.jpg"));

        [Command("bonk", RunMode = RunMode.Async)]
        [Summary("Posts 'Bonk' meme")]
        public async Task Bonk() => await PostToChannel(Path.Combine(env, "bonk.jpg"));

        [Command("buggin", RunMode = RunMode.Async)]
        [Summary("Posts 'Something bugging you?' meme")]
        public async Task Buggin() => await PostToChannel(Path.Combine(env, "buggin.png"));

        [Alias("chilling", "koopa")]
        [Command("chillin", RunMode = RunMode.Async)]
        [Summary("Posts 'chilling' meme")]
        public async Task Chillin() => await PostToChannel(Path.Combine(env, "chillin.jpg"));

        [Command("fireemblem", RunMode = RunMode.Async)]
        [Summary("Posts 'war crimes' meme")]
        public async Task FireEmblem() => await PostToChannel(Path.Combine(env, "fireemblem.png"));

        [Command("home", RunMode = RunMode.Async)]
        [Summary("Posts 'I wanna go home' meme")]
        public async Task Home() => await PostToChannel(Path.Combine(env, "home.jpg"));

        [Command("pog", RunMode = RunMode.Async)]
        [Summary("Posts 'pog' meme")]
        public async Task Pog() => await PostToChannel(Path.Combine(env, "pog.jpg"));

        [Command("shez", RunMode = RunMode.Async)]
        [Summary("Posts 'shez!' meme")]
        public async Task Shez() =>
            await PostToChannel(
                _randomizerService.GetRandom(2) == 0
                    ? Path.Combine(env, "shez1.jpg")
                    : Path.Combine(env, "shez2.jpg")
            );

        [Command("shock", RunMode = RunMode.Async)]
        [Summary("Posts 'shock' meme")]
        public async Task Shock() => await PostToChannel(Path.Combine(env, "shock.jpg"));

        [Command("stare", RunMode = RunMode.Async)]
        [Summary("Posts 'stare' meme")]
        public async Task Stare() => await PostToChannel(Path.Combine(env, "stare.gif"));

        [Command("xenoblade", RunMode = RunMode.Async)]
        [Summary("Posts 'Holy s* it's the xenoblade' meme")]
        public async Task Xenoblade() => await PostToChannel(Path.Combine(env, "xenoblade.png"));

        private async Task PostToChannel(string filePathToPost) =>
            await Context.Channel.SendFileAsync(filePathToPost);
    }
}
