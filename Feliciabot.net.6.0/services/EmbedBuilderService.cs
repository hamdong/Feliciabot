using Discord;
using Lavalink4NET.Tracks;

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

        internal static Embed GetBotInfoAsEmbed(string botInfo, string serverInfo)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", botInfo);
            builder.AddField("Server Info", serverInfo);
            builder.WithThumbnailUrl("https://raw.githubusercontent.com/Andu2/FEH-Mass-Simulator/master/heroes/Felicia.png");
            builder.WithColor(Color.LightGrey);
            return builder.Build();
        }

        internal Embed GetTrackInfoAsEmbed(LavalinkTrack track)
        {
            var builder = new EmbedBuilder();
            string trackUri = track.Uri is null ? "" : track.Uri.AbsoluteUri;
            string artworkUri = track.ArtworkUri is null ? "" : track.ArtworkUri.AbsoluteUri;
            TimeSpan position = (TimeSpan)(track.StartPosition is null ? new TimeSpan() : track.StartPosition);

            builder.WithAuthor(track.Author, MARIANNE_DANCE_LINK, trackUri);
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Uri}");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.AddField("Remaining", (track.Duration - position).ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(artworkUri);
            return builder.Build();
        }
    }
}
