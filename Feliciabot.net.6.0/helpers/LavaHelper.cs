using Discord;
using Victoria;
using Victoria.Player;
using Victoria.Responses.Search;

namespace Feliciabot.net._6._0.helpers
{
    internal static class LavaHelper
    {
        public static bool IsUserInVoiceChannel(IUser user)
        {
            var voiceState = user as IVoiceState;
            return (voiceState?.VoiceChannel != null);
        }

        public static bool IsSearchResponseAPlaylist(SearchResponse searchResponse)
        {
            return (!string.IsNullOrEmpty(searchResponse.Playlist.Name));
        }

        public static bool IsPlayerInUse(LavaPlayer<LavaTrack> player)
        {
            return (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused);
        }

        /// <summary>
        /// Responds with an embed build containing the specified track's information
        /// </summary>
        /// <param name="track">Track to display information on</param>
        /// <returns>Embed build containing track information</returns>
        public static async Task<Embed> GetTrackInfoAsEmbedAsync(LavaTrack track)
        {
            var builder = new EmbedBuilder();
            string art = await track.FetchArtworkAsync();

            builder.WithAuthor(track.Author, CommandsHelper.MARIANNE_DANCE_LINK, track.Url);
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Url}");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.AddField("Remaining", (track.Duration - track.Position).ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(art);
            return builder.Build();
        }
    }
}
