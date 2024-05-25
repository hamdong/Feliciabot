using Discord;
using Lavalink4NET.Tracks;

namespace Feliciabot.net._6._0.services
{
    public class EmbedBuilderService
    {
        private readonly string MARIANNE_DANCE_LINK = "https://cdn.discordapp.com/emojis/899319530269061161.gif";
        private readonly EmbedBuilder builder;

        public EmbedBuilderService()
        {
            builder = new EmbedBuilder();
        }

        internal Embed GetTestEmbed()
        {
            builder.WithTitle("Example Embed")
                .WithDescription("This is an example embed.")
                .WithColor(Color.Blue)
                .WithTimestamp(DateTimeOffset.UtcNow);

            builder.AddField("Field Name", "Field Value", true);
            return builder.Build();
        }

        internal Embed GetTrackInfoAsEmbed(LavalinkTrack track)
        {
            builder.WithAuthor(track.Author, MARIANNE_DANCE_LINK, track.Uri.AbsoluteUri);
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Uri}");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(track.ArtworkUri.AbsoluteUri);
            return builder.Build();
        }
    }
}
