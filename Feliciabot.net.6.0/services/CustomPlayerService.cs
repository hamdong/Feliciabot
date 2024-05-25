using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace Feliciabot.net._6._0.services
{
    public sealed class CustomPlayerService : QueuedLavalinkPlayer, IInactivityPlayerListener
    {
        public CustomPlayerService(IPlayerProperties<CustomPlayerService, CustomPlayerOptions> properties)
            : base(properties)
        {
        }

        public ValueTask NotifyPlayerActiveAsync(CancellationToken cancellationToken = default)
        {
            // This method is called when the player was previously inactive and is now active again.
            // For example: All users in the voice channel left and now a user joined the voice channel again.
            cancellationToken.ThrowIfCancellationRequested();
            return default; // do nothing
        }

        public ValueTask NotifyPlayerActiveAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask NotifyPlayerInactiveAsync(CancellationToken cancellationToken = default)
        {
            // This method is called when the player reached the inactivity deadline.
            // For example: All users in the voice channel left and the player was inactive for longer than 30 seconds.
            cancellationToken.ThrowIfCancellationRequested();
            return default; // do nothing
        }

        public ValueTask NotifyPlayerInactiveAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask NotifyPlayerTrackedAsync(CancellationToken cancellationToken = default)
        {
            // This method is called when the player was previously active and is now inactive.
            // For example: A user left the voice channel and now all users left the voice channel.
            cancellationToken.ThrowIfCancellationRequested();
            return default; // do nothing
        }

        public ValueTask NotifyPlayerTrackedAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
