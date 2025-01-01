using Discord;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using Lavalink4NET.Players;
using Stax.GetAverageImageColor.GetAverageImageColor;

namespace Feliciabot.net._6._0.services
{
    public class EmbedBuilderService(IRandomizerService _randomizerService) : IEmbedBuilderService
    {
        internal static Embed GetBotInfoAsEmbed(string botInfo)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", botInfo);
            builder.WithThumbnailUrl(
                "https://raw.githubusercontent.com/Andu2/FEH-Mass-Simulator/master/heroes/Felicia.png"
            );
            builder.WithColor(Color.LightGrey);
            return builder.Build();
        }

        public async Task<Embed> GetTrackInfoFromPlayerAsEmbed(
            LavalinkPlayer player,
            bool skipped = false
        )
        {
            var builder = new EmbedBuilder();
            var track = player.CurrentTrack;

            if (track is null)
                return builder.Build();

            string trackUri = track.Uri is null ? "" : track.Uri.AbsoluteUri;
            string artworkUri = track.ArtworkUri is null ? "" : track.ArtworkUri.AbsoluteUri;
            string colorCode = await GetImageColor.AverageFromUrl(artworkUri);

            var colorParsed = Color.TryParse(colorCode[..7], out Color color);
            var pos = skipped ? new TimeSpan() : player.Position?.Position ?? new TimeSpan();
            string format = track.Duration >= TimeSpan.FromHours(1) ? "hh\\:mm\\:ss" : "mm\\:ss";
            var trackPosition =
                $"{pos.ToString(format)} {GetPosVisual(pos, track.Duration)} {track.Duration.ToString(format)}";

            builder.WithAuthor(
                track.Author,
                Responses.DanceResponses[
                    _randomizerService.GetRandom(Responses.DanceResponses.Length)
                ],
                trackUri
            );
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Uri}" ?? "[no URL found]");
            builder.AddField("Track Position", $"{trackPosition}", true);
            builder.WithThumbnailUrl(artworkUri);
            builder.WithColor(colorParsed ? color : Color.Default);
            builder.WithFooter(track.SourceName ?? "[no source found]");
            return builder.Build();
        }

        private static string GetPosVisual(TimeSpan position, TimeSpan duration)
        {
            double progressPercentage = (position.TotalSeconds / duration.TotalSeconds) * 100;
            int filledCharacters = (int)(progressPercentage / 5);

            string progressBar = new('#', filledCharacters);
            string emptyBar = new('-', 20 - filledCharacters); // max 20 char length
            return $"[{progressBar}{emptyBar}]";
        }
    }
}
