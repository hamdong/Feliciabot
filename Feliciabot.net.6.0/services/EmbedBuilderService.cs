using Discord;
using Lavalink4NET.Players;

namespace Feliciabot.net._6._0.services
{
    public class EmbedBuilderService
    {
        private readonly string MARIANNE_DANCE_LINK = "https://cdn.discordapp.com/emojis/899319530269061161.gif";

        internal static Embed GetTestEmbed()
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("Example Embed")
                .WithDescription("This is an example embed.")
                .WithColor(Color.Blue)
                .WithTimestamp(DateTimeOffset.UtcNow);

            builder.AddField("Field Name", "Field Value", true);
            return builder.Build();
        }

        internal static Embed GetBotInfoAsEmbed(string botInfo)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", botInfo);
            builder.WithThumbnailUrl("https://raw.githubusercontent.com/Andu2/FEH-Mass-Simulator/master/heroes/Felicia.png");
            builder.WithColor(Color.LightGrey);
            return builder.Build();
        }

        internal Embed GetPlayingTrackInfoAsEmbed(LavalinkPlayer player)
        {
            var builder = new EmbedBuilder();
            var track = player.CurrentTrack;

            if (track is null) return builder.Build();

            string trackUri = track.Uri is null ? "" : track.Uri.AbsoluteUri;
            string artworkUri = track.ArtworkUri is null ? "" : track.ArtworkUri.AbsoluteUri;
            var position = player.Position?.Position ?? new TimeSpan();
            var visual = GetPositionVisual(position, track.Duration);
            var displayPosition = position.ToString("hh\\:mm\\:ss");
            var displayDuration = track.Duration.ToString("hh\\:mm\\:ss");

            builder.WithAuthor(track.Author, MARIANNE_DANCE_LINK, trackUri);
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Uri}");
            builder.AddField("Track Position", $"{displayPosition} {visual} {displayDuration}", true);
            builder.WithThumbnailUrl(artworkUri);
            return builder.Build();
        }

        private static string GetPositionVisual(TimeSpan position, TimeSpan duration)
        {
            double progressPercentage = (position.TotalSeconds / duration.TotalSeconds) * 100;
            int filledCharacters = (int)(progressPercentage / 5);

            string progressBar = new('#', filledCharacters);
            string emptyBar = new('-', 20 - filledCharacters); // max 20 char length
            return $"[{progressBar}{emptyBar}]";
        }
    }
}
