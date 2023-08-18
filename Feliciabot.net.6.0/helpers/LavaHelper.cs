using Discord;
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
    }
}
