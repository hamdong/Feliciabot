﻿using Discord.Interactions;
using Feliciabot.net._6._0.services;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;

namespace Feliciabot.net._6._0.modules
{
    [RequireContext(ContextType.Guild)]
    public sealed class MusicModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAudioService _audioService;
        private readonly EmbedBuilderService _embedBuilderService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MusicModule"/> class.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
        /// </exception>
        public MusicModule(IAudioService audioService, EmbedBuilderService embedBuilderService)
        {
            ArgumentNullException.ThrowIfNull(audioService);

            _audioService = audioService;
            _embedBuilderService = embedBuilderService;
        }

        /// <summary>
        ///     Disconnects from the current voice channel connected to asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
        public async Task Disconnect()
        {
            var player = await GetPlayerAsync().ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            await player.DisconnectAsync().ConfigureAwait(false);
            await RespondAsync("Disconnected.").ConfigureAwait(false);
        }

        /// <summary>
        ///     Plays music asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
        public async Task Play(string query)
        {
            await DeferAsync().ConfigureAwait(false);

            var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            var track = await _audioService.Tracks
                .LoadTrackAsync(query, TrackSearchMode.YouTube)
                .ConfigureAwait(false);

            if (track is null)
            {
                await FollowupAsync("😖 No results.").ConfigureAwait(false);
                return;
            }

            var position = await player.PlayAsync(track).ConfigureAwait(false);

            if (position is 0)
            {
                await FollowupAsync($"🔈 Playing:", embed: _embedBuilderService.GetTrackInfoAsEmbed(track)).ConfigureAwait(false);
            }
            else
            {
                await FollowupAsync($"🔈 Added to queue: {track.Title}").ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Shows the track position asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
        public async Task Position()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            if (player.CurrentItem is null)
            {
                await RespondAsync("Nothing playing!").ConfigureAwait(false);
                return;
            }

            await RespondAsync($"Position: {player.Position?.Position} / {player.CurrentTrack.Duration}.").ConfigureAwait(false);
        }

        /// <summary>
        ///     Stops the current track asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
        public async Task Stop()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false);

            if (player is null)
            {
                return;
            }

            if (player.CurrentItem is null)
            {
                await RespondAsync("Nothing playing!").ConfigureAwait(false);
                return;
            }

            await player.StopAsync().ConfigureAwait(false);
            await RespondAsync("Stopped playing.").ConfigureAwait(false);
        }

        [SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
        public async Task Skip()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false);

            if (player is null)
            {
                return;
            }

            if (player.CurrentItem is null)
            {
                await RespondAsync("Nothing playing!").ConfigureAwait(false);
                return;
            }

            await player.SkipAsync().ConfigureAwait(false);

            var track = player.CurrentItem;

            if (track is not null)
            {
                await RespondAsync($"Skipped. Now playing: {track.Track!.Uri}", embed: _embedBuilderService.GetTrackInfoAsEmbed(track.Track)).ConfigureAwait(false);
            }
            else
            {
                await RespondAsync("Skipped. Stopped playing because the queue is now empty.").ConfigureAwait(false);
            }
        }

        [SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
        public async Task PauseAsync()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false);

            if (player is null)
            {
                return;
            }

            if (player.State is PlayerState.Paused)
            {
                await RespondAsync("Player is already paused.").ConfigureAwait(false);
                return;
            }

            await player.PauseAsync().ConfigureAwait(false);
            await RespondAsync("Paused.").ConfigureAwait(false);
        }

        [SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
        public async Task ResumeAsync()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false);

            if (player is null)
            {
                return;
            }

            if (player.State is not PlayerState.Paused)
            {
                await RespondAsync("Player is not paused.").ConfigureAwait(false);
                return;
            }

            await player.ResumeAsync().ConfigureAwait(false);
            await RespondAsync("Resumed.").ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the guild player asynchronously.
        /// </summary>
        /// <param name="connectToVoiceChannel">
        ///     a value indicating whether to connect to a voice channel
        /// </param>
        /// <returns>
        ///     a task that represents the asynchronous operation. The task result is the lavalink player.
        /// </returns>
        private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
        {
            var channelBehavior = connectToVoiceChannel
                ? PlayerChannelBehavior.Join
                : PlayerChannelBehavior.None;

            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);
            var result = await _audioService.Players
                .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions)
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                    _ => "Unknown error.",
                };

                await RespondAsync(errorMessage).ConfigureAwait(false);
                return null;
            }

            return result.Player;
        }
    }
}
