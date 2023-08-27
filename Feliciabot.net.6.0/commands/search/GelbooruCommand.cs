using BooruSharp.Booru;
using Discord;
using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Command for searching via Gelbooru API
    /// </summary>
    public class GelbooruCommand : ModuleBase
    {
        private const int MAX_IMG_HISTORY = 20;
        private const string RATING_SAFE = "rating:general";
        private const string RATING_EXPLICIT = "rating:explicit";
        private const string ARTIST_THOR = "thor_(deep_rising)";
        private const string ARTIST_BORIS = "boris_(noborhys)";

        private readonly static List<string> searchHistory = new();
        private readonly string[] cursedTags = new string[] { "-loli", "-guro", "-fart", "-rape", "-netorare", "-furry", "-my_little_pony", "-vore,", "-piss", "-beastiality", "-veiny_penis", "-netori" };
        private readonly Gelbooru _booru;

        public GelbooruCommand(Gelbooru booru)
        {
            _booru = booru;
        }

        /// <summary>
        /// Safe Search
        /// </summary>
        /// <param name="tag"></param>
        [Command("safe", RunMode = RunMode.Async), Summary("Shows a random sfw Gelbooru image. If no search tags, it defaults to Felicia. (for multiple tags, use spaces) [Usage]: !safe fire_emblem sword, !awawa")]
        [Alias("awawa")]
        public async Task SafeSearch([Summary("Image tag."), Remainder] string tag = "") => await ImgSearch(tag, RATING_SAFE);

        /// <summary>
        /// NSFW Search
        /// </summary>
        /// <param name="tag"></param>
        [Command("lewd", RunMode = RunMode.Async), Summary("Shows a random nsfw Gelbooru image. If no search tags, it defaults to Felicia. (for multiple tags, use spaces) [Usage]: !lewd fire_emblem sword [Note]: only use in nsfw channel")]
        public async Task LewdSearch([Summary("Image tag."), Remainder] string tag = "") => await ImgSearch(tag, RATING_EXPLICIT);

        /// <summary>
        /// Search for images by thor_(deep_rising)
        /// </summary>
        /// <param name="tag"></param>
        [Command("thor", RunMode = RunMode.Async), Summary("Shows a random nsfw Gelbooru image by the artist Thor (Deep Rising). Use tags for refined searches. (for multiple tags, use spaces) [Usage]: !thor my_unit_(fire_emblem_if) [Note]: only use in nsfw channel")]
        public async Task ThorSearch([Summary("Image tag."), Remainder] string tag = "")
        {
            tag += !tag.Contains(ARTIST_THOR) ? " " + ARTIST_THOR : "";
            await ImgSearch(tag, RATING_EXPLICIT);
        }

        /// <summary>
        /// Search for images by boris_(noborhys)
        /// </summary>
        /// <param name="tag"></param>
        [Command("boris", RunMode = RunMode.Async), Summary("Shows a random nsfw Gelbooru image by the artist Boris (noborhys). Use tags for refined searches. (for multiple tags, use spaces) [Usage]: !boris my unit (fire emblem if) [Note]: only use in nsfw channel")]
        public async Task BorisSearch([Summary("Image tag."), Remainder] string tag = "")
        {
            tag += !tag.Contains(ARTIST_THOR) ? " " + ARTIST_BORIS : "";
            await ImgSearch(tag, RATING_EXPLICIT);
        }

        private async Task ImgSearch(string tag, string rating = "")
        {
            string removeThor = "";
            string remmoveBoris = "";

            if (rating == RATING_EXPLICIT && !((ITextChannel)Context.Channel).IsNsfw)
            {
                await Context.Channel.SendMessageAsync("You can't use that here! :angry:");
                return;
            }

            // Default to Felicia
            if (tag == "")
            {
                tag = "felicia_(fire_emblem)";
            }

            // Ignore thor unless requested
            if (!tag.Contains(ARTIST_THOR))
            {
                removeThor = "-" + ARTIST_THOR;
            }

            // Ignore boris unless requested
            if (!tag.Contains(ARTIST_BORIS))
            {
                remmoveBoris = "-" + ARTIST_BORIS;
            }

            // Omit all cursed tags unless specifically searched for
            for (int i = 0; i < cursedTags.Length; i++)
            {
                if (tag.Contains(cursedTags[i]))
                {
                    cursedTags[i] = "";
                }
            }

            try
            {
                // Img search
                BooruSharp.Search.Post.SearchResult result = await _booru.GetRandomPostAsync(tag,
                    cursedTags[0], cursedTags[1], cursedTags[2], cursedTags[3], cursedTags[4], cursedTags[5], cursedTags[6], cursedTags[7], cursedTags[8], cursedTags[9], cursedTags[10],
                    remmoveBoris, removeThor, rating);
                string imgURL = result.FileUrl.ToString();

                //Track search history to prevent repeats
                for (int i = 0; i < MAX_IMG_HISTORY; i++)
                {
                    // Retry if exists
                    if (searchHistory.Contains(imgURL))
                        result = await _booru.GetRandomPostAsync(tag,
                            cursedTags[0], cursedTags[1], cursedTags[2], cursedTags[3], cursedTags[4], cursedTags[5], cursedTags[6], cursedTags[7], cursedTags[8], cursedTags[9], cursedTags[10],
                            remmoveBoris, removeThor, rating);
                    else
                        break;
                }
                searchHistory.Add(imgURL);

                //Remove oldest item in search history
                if (searchHistory.Count > MAX_IMG_HISTORY)
                {
                    searchHistory.RemoveAt(0);
                }

                await Context.Channel.SendMessageAsync(":heart: Gelbooru: " + result.FileUrl);
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("No results found for tag:" + tag + ".");
            }
        }
    }
}
