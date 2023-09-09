using Discord;
using Feliciabot.net._6._0.helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace Feliciabot.net._6._0.services
{
    public sealed class AudioService
    {
        private readonly LavaNode _lavaNode;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

        public AudioService(LavaNode lavaNode, ILoggerFactory loggerFactory)
        {
            _lavaNode = lavaNode;
            _logger = loggerFactory.CreateLogger<LavaNode>();
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

            _lavaNode.OnTrackEnd += OnTrackEndAsync;
            _lavaNode.OnTrackStart += OnTrackStartAsync;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosedAsync;
            _lavaNode.OnTrackStuck += OnTrackStuckAsync;
            _lavaNode.OnTrackException += OnTrackExceptionAsync;
        }

        /// <summary>
        /// Event triggers whenever a track ends, used for checking if the bot should initiate an auto disconnect
        /// </summary>
        /// <param name="args">Arguments containing info on the status of what caused the track end event</param>
        private async Task OnTrackEndAsync(TrackEndEventArg<LavaPlayer<LavaTrack>, LavaTrack> args)
        {
            var player = args.Player;

            if (args.Reason == TrackEndReason.Replaced)
            {
                return;
            }

            // If the track was stopped, commence auto-disconnect if the queue is empty
            if (args.Reason == TrackEndReason.Stopped)
            {
                if (player.Vueue == null || !player.Vueue.Any())
                {
                    await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks for more music!");
                    _ = InitiateDisconnectAsync(player, TimeSpan.FromMinutes(5));
                }
                return;
            }

            // Clear the track from the queue upon it ending
            if (!player.Vueue.TryDequeue(out var currentTrack))
            {
                await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks for more music!");
                _ = InitiateDisconnectAsync(player, TimeSpan.FromMinutes(5));
                return;
            }

            if (currentTrack is null)
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await player.PlayAsync(currentTrack);
            await player.TextChannel.SendMessageAsync($"{args.Reason}: {args.Track.Title}\nNow playing:", false, await LavaHelper.GetTrackInfoAsEmbedAsync(player.Track));
        }

        /// <summary>
        /// Event triggers whenever a track starts
        /// </summary>
        /// <param name="arg">Arguments containing info on the status of what caused the track start event</param>
        private async Task OnTrackStartAsync(TrackStartEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
        {
            var player = arg.Player;
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value)) return;
            if (value.IsCancellationRequested) return;

            value.Cancel(true);
            await player.TextChannel.SendMessageAsync("Auto disconnect has been cancelled!");
        }

        /// <summary>
        /// Call to disconnect the bot from the connect voice channel
        /// </summary>
        /// <param name="player">Music player object to disconnect</param>
        /// <param name="timeSpan">Timespan to specify delay before disconnect</param>
        private async Task InitiateDisconnectAsync(LavaPlayer<LavaTrack> player, TimeSpan timeSpan)
        {
            // client was forcibly disconnected
            if (player.VoiceChannel is null)
            {
                if (player.TextChannel != null)
                {
                    await player.TextChannel.SendMessageAsync($"Successfully disconnected from voice channel.");
                }
                return;
            }

            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value))
            {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested)
            {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await player.TextChannel.SendMessageAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...");
            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled)
            {
                return;
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);
            await player.TextChannel.SendMessageAsync($"Successfully disconnected from voice channel.");
        }

        /// <summary>
        /// Call to log exception if track throws an exception
        /// </summary>
        /// <param name="arg">exception argument containing information</param>
        private async Task OnTrackExceptionAsync(TrackExceptionEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
        {
            var player = arg.Player;
            await player.TextChannel.SendMessageAsync($"Track {arg.Track.Title} threw an exception. Exception has been logged to console.");
            LogMessage msg = new LogMessage(LogSeverity.Error, arg.GetType().ToString(), arg.Exception.Message);
            await Main.LogHandler(msg);
            player.Vueue.Enqueue(arg.Track);
            await player.TextChannel.SendMessageAsync($"{arg.Track.Title} has been re-added to queue after throwing an exception.");
        }

        /// <summary>
        /// Call to log exception if track is stuck
        /// </summary>
        /// <param name="arg">exception argument containing information</param>
        private async Task OnTrackStuckAsync(TrackStuckEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
        {
            var player = arg.Player;
            await player.TextChannel.SendMessageAsync($"Track {arg.Track.Title} got stuck for {arg.Threshold}ms.");
            player.Vueue.Enqueue(arg.Track);
            await player.TextChannel.SendMessageAsync($"{arg.Track.Title} has been re-added to queue after getting stuck.");
        }

        /// <summary>
        /// Call to log exception if web socket connection is lost
        /// </summary>
        /// <param name="arg">exception argument containing information</param>
        private async Task OnWebSocketClosedAsync(WebSocketClosedEventArg arg)
        {
            LogMessage msg = new(LogSeverity.Error, arg.GetType().ToString(), $"Discord WebSocket connection closed with the following reason: {arg.Reason}");
            _ = Main.LogHandler(msg);

            IGuild currentGuild = arg.Guild;
            var getPlayerSuccess = _lavaNode.TryGetPlayer(currentGuild, out LavaPlayer<LavaTrack> player);
            if (getPlayerSuccess)
            {
                var textChannel = player.TextChannel;
                await textChannel.SendMessageAsync($"Discord WebSocket connection closed with the following reason: {arg.Reason}");
                await _lavaNode.LeaveAsync(player.VoiceChannel);
            }
        }
    }
}
