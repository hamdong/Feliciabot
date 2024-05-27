using Discord;
using Discord.Interactions;
using Feliciabot.net._6._0.services;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;

namespace Feliciabot.net._6._0.modules
{
    [RequireContext(ContextType.Guild)]
    public sealed class MusicModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAudioService _audioService;
        private readonly EmbedBuilderService _embedBuilderService;
        private readonly InteractiveService _interactiveService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MusicModule"/> class.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
        /// </exception>
        public MusicModule(IAudioService audioService, EmbedBuilderService embedBuilderService, InteractiveService interactiveService)
        {
            ArgumentNullException.ThrowIfNull(audioService);

            _audioService = audioService;
            _embedBuilderService = embedBuilderService;
            _interactiveService = interactiveService;
        }

        /// <summary>
        ///     Disconnects from the current voice channel connected to asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("leave", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
        public async Task Leave()
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
        ///     Displays the current playing track
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("now-playing", description: "Displays the currently playing track", runMode: RunMode.Async)]
        public async Task NowPlaying()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            if (player.CurrentTrack == null || player.IsPaused)
            {
                await RespondAsync($"Nothing is playing currently.").ConfigureAwait(false);
                return;
            }

            var track = player.CurrentTrack;
            await RespondAsync("Now playing:", embed: _embedBuilderService.GetTrackInfoAsEmbed(track)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Display the current queue.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [SlashCommand("queue", description: "Shows the current queue", runMode: RunMode.Async)]
        public async Task Queue()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            if (player.Queue is null || player.Queue.Count == 0)
            {
                await RespondAsync("Queue is empty!").ConfigureAwait(false);
                return;
            }

            var paginatedQueue = GetPaginatedQueue(player.Queue, out TimeSpan totalDuration);
            await RespondAsync($"Total Duration: {totalDuration}").ConfigureAwait(false);
            await _interactiveService.SendPaginatorAsync(paginatedQueue, Context.Channel, TimeSpan.FromMinutes(5));
        }

        [SlashCommand("clear", description: "Clears the current queue", runMode: RunMode.Async)]
        public async Task Clear()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            if (player.Queue is null || player.Queue.Count == 0)
            {
                await RespondAsync("Queue is empty!").ConfigureAwait(false);
                return;
            }

            int cleared = await player.Queue.ClearAsync();
            await RespondAsync($"Queue cleared! Removed items: {cleared}").ConfigureAwait(false);
        }

        [SlashCommand("remove", description: "Removes an item from the current queue", runMode: RunMode.Async)]
        public async Task Remove(int trackNumber)
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            if (player.Queue is null || player.Queue.Count == 0)
            {
                await RespondAsync("Queue is empty!").ConfigureAwait(false);
                return;
            }

            if (trackNumber < 1 || trackNumber > player.Queue.Count)
            {
                await RespondAsync("Invalid track number!").ConfigureAwait(false);
                return;
            }

            int trackCount = 1;
            ITrackQueueItem? trackToRemove = null;
            foreach (ITrackQueueItem track in player.Queue)
            {
                if (trackCount == trackNumber)
                {
                    trackToRemove = track;
                    break;
                }
                trackCount++;
            }

            if (trackToRemove != null && trackToRemove.Track != null)
            {
                await player.Queue.RemoveAsync(trackToRemove);
                await RespondAsync($"Removed track: '{trackCount}. {trackToRemove.Track.Title}'").ConfigureAwait(false);
            }
            else
            {
                await RespondAsync($"Unable to remove track number ({trackNumber})").ConfigureAwait(false);
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
        private async ValueTask<CustomPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
        {
            var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var textChannel = Context.Channel as ITextChannel ?? Context.Guild.TextChannels.FirstOrDefault();
            var playerOptions = new CustomPlayerOptions(textChannel);

            static ValueTask<CustomPlayer> CreatePlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ValueTask.FromResult(new CustomPlayer(properties));
            }

            var result = await _audioService.Players
                .RetrieveAsync<CustomPlayer, CustomPlayerOptions>(Context, playerFactory: CreatePlayer, options: Options.Create(playerOptions), retrieveOptions)
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                    _ => "Unknown error.",
                };

                await FollowupAsync(errorMessage).ConfigureAwait(false);
                return null;
            }

            if (Context.User is not IVoiceState voiceState)
            {
                await FollowupAsync($"I can't find a voice channel.").ConfigureAwait(false);
                return null;
            }

            if (voiceState.VoiceChannel is null || result.Player.VoiceChannelId != voiceState.VoiceChannel.Id)
            {
                await FollowupAsync($"You must be in the same voice channel as me!").ConfigureAwait(false);
                return null;
            }
            return result.Player;
        }

        private StaticPaginator GetPaginatedQueue(ITrackQueue queue, out TimeSpan totalDuration)
        {
            List<string> trackList = [];
            string pageContent = string.Empty;
            int trackNum = 1;
            TimeSpan ongoingDuration = new();
            int trackOnPageCount = 1;

            foreach (ITrackQueueItem t in queue)
            {
                var track = t.Track;

                if (track is null) continue;
                var uri = track.Uri is null ? "" : track.Uri.AbsoluteUri;

                pageContent += $"{trackNum}. [{track.Title}]({uri}) [{track.Duration}]\n\n";

                if (trackOnPageCount % 5 != 0)
                {
                    trackOnPageCount++;
                }
                else
                {
                    trackList.Add(pageContent);
                    pageContent = string.Empty;
                    trackOnPageCount = 1;
                }
                ongoingDuration += track.Duration;
                trackNum++;
            }

            if (pageContent != string.Empty)
            {
                trackList.Add(pageContent);
            }

            var pages = trackList.ToArray();
            List<PageBuilder> pagebuilder = [];

            foreach (string page in pages)
            {
                pagebuilder.Add(new PageBuilder().WithDescription(page));
            }

            totalDuration = ongoingDuration;
            return new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only interacted user can parse pages
                .WithPages(pagebuilder)
                .Build();
        }
    }
}
