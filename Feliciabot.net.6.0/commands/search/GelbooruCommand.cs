using BooruSharp.Booru;
using Discord;
using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    public class GelbooruCommand(Gelbooru booru) : ModuleBase
    {
        private const string RATING_SAFE = "rating:general";
        private const string RATING_EXPLICIT = "rating:explicit";
        private readonly string[] cursedTags = ["-loli", "-guro", "-fart", "-rape", "-netorare", "-furry", "-my_little_pony", "-vore", "-piss", "-beastiality", "-veiny_penis", "-netori"];
        private readonly Gelbooru _booru = booru;

        /// <summary>
        /// Safe Search
        /// </summary>
        /// <param name="tag"></param>
        [Command("safe", RunMode = RunMode.Async), Summary("Posts a random sfw Gelbooru image (default: Felicia), use spaces for multiple tags")]
        [Alias("awawa")]
        public async Task SafeSearch([Summary("Image tag."), Remainder] string tag = "") => await ImgSearch(tag, RATING_SAFE);

        /// <summary>
        /// NSFW Search
        /// </summary>
        /// <param name="tag"></param>
        [Command("lewd", RunMode = RunMode.Async), Summary("Posts a random nsfw Gelbooru image in nsfw channel (default: Felicia), use spaces for multiple tags")]
        public async Task LewdSearch([Summary("Image tag."), Remainder] string tag = "") => await ImgSearch(tag, RATING_EXPLICIT);

        private async Task ImgSearch(string tag, string rating)
        {
            if (rating == RATING_EXPLICIT && !((ITextChannel)Context.Channel).IsNsfw)
            {
                await Context.Channel.SendMessageAsync("You can't use that here! :angry:");
                return;
            }

            if (tag == "")
            {
                tag = "felicia_(fire_emblem)";
            }

            try
            {
                var searchQuery = new string[] { tag, string.Join(", ", cursedTags), rating };
                BooruSharp.Search.Post.SearchResult result = await _booru.GetRandomPostAsync(searchQuery);

                await Context.Channel.SendMessageAsync($":heart: Gelbooru: { result.FileUrl}");
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync($"No results found for tag: {tag}.");
            }
        }
    }
}
