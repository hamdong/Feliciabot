using Discord.Commands;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Services;

namespace Feliciabot.net._6._0.commands.search
{
    public class Youtube(YoutubeSearchClient ytClient, InteractiveService interactiveService) : ModuleBase
    {
        [Command("youtube", RunMode = RunMode.Async), Alias("yt"), Summary("Performs a Youtube search and returns the first result")]
        public async Task SingleSearch([Remainder] string query)
        {
            var response = await ytClient.SearchAsync(query);
            if (response.Results.Count == 0)
            {
                await ReplyAsync($"I wasn't able to find any results for `{query}`. :confused:");
                return;
            }

            await ReplyAsync(response.Results.First().Url);
        }

        [Command("youtubelist", RunMode = RunMode.Async), Alias("ytl"), Summary("Performs a Youtube search and returns all results in an embedded list")]
        public async Task MultiSearch([Remainder] string query)
        {
            var response = await ytClient.SearchAsync(query);

            if (response.Results.Count == 0)
            {
                await ReplyAsync($"I wasn't able to find any results for `{query}`. :confused:");
                return;
            }

            var pages = response.Results.ToArray();
            List<PageBuilder> pagebuilder = [];

            foreach (YoutubeVideo page in pages)
            {
                pagebuilder.Add(new PageBuilder()
                    .WithTitle(page.Title)
                    .WithDescription(page.Url)
                    .WithThumbnailUrl(page.ThumbnailUrl));
            }

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only the user that executed the command can interact
                .WithPages(pagebuilder)
                .Build();

            await interactiveService.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(5));
        }
    }

}
