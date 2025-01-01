using Discord;
using Lavalink4NET.Players;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IEmbedBuilderService
    {
        public Task<Embed> GetTrackInfoFromPlayerAsEmbed(
            LavalinkPlayer player,
            bool skipped = false
        );
    }
}
