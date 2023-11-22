using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using Victoria.Node;
using Victoria.Player;
using Victoria.Responses.Search;

namespace Feliciabot.net._6._0.commands
{
    public class MusicCommand : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private const int NUM_TRACKS_PER_PAGE = 10;
        private readonly InteractiveService _interactiveService;

        /// <summary>
        /// Constructor for Music command
        /// </summary>
        /// <param name="lavaNode">client for the lavaNode api</param>
        /// <param name="interactiveService">interactive service functionality</param>
        public MusicCommand(LavaNode lavaNode, InteractiveService interactiveService)
        {
            _lavaNode = lavaNode;
            _interactiveService = interactiveService;
        }

        /// <summary>
        /// Checks the status of the music player connection
        /// </summary>
        [Command("musicplayerstatus", RunMode = RunMode.Async)]
        [Summary("Checks the status of the music player connection. [Usage] !musicplayerstatus")]
        public async Task MusicPlayerStatus()
        {
            await ReplyAsync(_lavaNode.IsConnected ? "Player is connected! :smiley:" : "Player is not connected! :scream:");
        }

        /// <summary>
        /// Joins the context call channel
        /// </summary>
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the context call channel. [Usage] !join")]
        public async Task JoinAsync()
        {
            if (!LavaHelper.IsUserInVoiceChannel(Context.User))
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var getPlayerSuccess = _lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer<LavaTrack> player);
                await ReplyAsync(getPlayerSuccess ? $"I've already joined in voice channel {player.VoiceChannel}." : $"Unable to get player :(!");
                return;
            }


            if (Context.User is not IVoiceState voiceState)
            {
                await ReplyAsync($"Unable to determine voice state (null). Cannot join channel.");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        /// <summary>
        /// Plays the current track
        /// </summary>
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Plays the current track. [Usage] !play")]
        public async Task Play()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Player is in invalid state
            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (player.PlayerState == PlayerState.Paused)
            {
                await player.ResumeAsync();
                await ReplyAsync("Resuming music.");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("A song is already playing!");
                await ReplyAsync("Now playing:", false, await LavaHelper.GetTrackInfoAsEmbedAsync(player.Track));
                return;
            }

            if (!player.Vueue.Any())
            {
                await ReplyAsync("Play what? There are no more songs in the queue!");
                return;
            }

            var nextTrack = await player.SkipAsync();
            await ReplyAsync("Now playing:", false, await LavaHelper.GetTrackInfoAsEmbedAsync(nextTrack.Current));
        }

        /// <summary>
        /// Queues up music and then plays the specified track or playlist
        /// </summary>
        /// <param name="query">Parameter to search for music based on title</param>
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Plays a track from a specified song via search query in the current call. [Usage] !play [query]")]
        public async Task Play([Remainder] string query)
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Search for music via Youtube or direct Uri
            var searchResponse = await _lavaNode.SearchAsync(Uri.IsWellFormedUriString(query, UriKind.Absolute) ? SearchType.Direct : SearchType.YouTube, query);
            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            // Check for a playlist and queue up music
            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Vueue.Enqueue(searchResponse.Tracks);
                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
            }
            else
            {
                var track = searchResponse.Tracks.First();
                player.Vueue.Enqueue(track);
                await ReplyAsync($"Enqueued {track?.Title}");
            }

            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return;
            }

            player.Vueue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(lavaTrack);
            await ReplyAsync("Now playing:", false, await LavaHelper.GetTrackInfoAsEmbedAsync(lavaTrack));
        }

        /// <summary>
        /// Displays the current remaining items in the queue
        /// </summary>
        [Command("queue", RunMode = RunMode.Async)]
        [Alias("playlist", "q")]
        [Summary("Displays the current remaining items in the queue. [Usage] !queue, !playlist, !q")]
        public async Task Queue()
        {

            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (!player.Vueue.Any())
            {
                await ReplyAsync("There are no more songs in the queue!");
                return;
            }

            List<string> trackList = new();
            TimeSpan totalDuration = new();
            string pageContent = string.Empty;
            int trackNum = 1;
            int trackOnPageCount = 1;

            foreach (LavaTrack track in player.Vueue)
            {
                pageContent += ($"{trackNum}. [{track.Title}]({track.Url}) [{track.Duration}]\n\n");
                if (trackOnPageCount % NUM_TRACKS_PER_PAGE != 0)
                {
                    trackOnPageCount++;
                }
                else
                {
                    trackList.Add(pageContent);
                    pageContent = string.Empty;
                    trackOnPageCount = 1;
                }
                totalDuration += track.Duration;
                trackNum++;
            }

            if (pageContent != string.Empty)
            {
                trackList.Add(pageContent);
            }

            // Create paginated message
            var pages = trackList.ToArray();
            List<PageBuilder> pagebuilder = new List<PageBuilder>();

            foreach (string page in pages)
            {
                pagebuilder.Add(new PageBuilder().WithDescription(page));
            }

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only allow the user that executed the command to interact with the selection.
                .WithPages(pagebuilder) // Set the pages the paginator will use. This is the only required component.
                .Build();

            // Send the paginator to the source channel and wait until it times out after 10 minutes.
            await _interactiveService.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// Removes a track from the queue based on the number
        /// </summary>
        /// <param name="trackNum">Number of the track to be removed</param>
        [Alias("rm")]
        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Removes numbered track from the queue. [Usage] !remove [track number], !rm")]
        public async Task Remove(int trackNum)
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Ensure the queue isn't empty
            if (!player.Vueue.Any())
            {
                await ReplyAsync("There are no songs in the queue to remove!");
                return;
            }

            // Ensure valid track number
            if (trackNum < 1 || trackNum > player.Vueue.Count)
            {
                await ReplyAsync("Invalid track number, value must be referenced in queue!");
                return;
            }

            // Get the track from the queue
            int trackCount = 1;
            LavaTrack? trackToRemove = null;
            foreach (LavaTrack track in player.Vueue)
            {
                if (trackCount == trackNum)
                {
                    trackToRemove = track;
                    break;
                }
                trackCount++;
            }

            // Remove the track
            if (trackToRemove != null)
            {
                player.Vueue.Remove(trackToRemove);
                await ReplyAsync($"Removed track: '{trackCount}. {trackToRemove.Title}'");
            }
            else
            {
                await ReplyAsync($"Unable to remove track number ({trackNum})");
            }
        }

        /// <summary>
        /// Skip the current playing track
        /// </summary>
        [Alias("fs")]
        [Command("skip", RunMode = RunMode.Async)]
        [Summary("Skips current track and plays the next one in the queue. [Usage] !skip, !fs")]
        public async Task Skip()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Player is in invalid state
            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (!player.Vueue.Any())
            {
                await ReplyAsync("Cannot skip when there are no more songs in the queue!");
                return;
            }

            try
            {
                var (skipped, current) = await player.SkipAsync();
                await ReplyAsync($"Skipped: {skipped.Title}\nNow playing: {current.Title}", false, await LavaHelper.GetTrackInfoAsEmbedAsync(current));
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        /// <summary>
        /// Pause the current playing track
        /// </summary>
        [Command("pause", RunMode = RunMode.Async)]
        [Summary("Pauses the currently playing track. [Usage] !pause")]
        public async Task Pause()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Player is in invalid state
            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (player.PlayerState == PlayerState.Paused)
            {
                await ReplyAsync("The music is already paused!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Paused music.");
        }

        /// <summary>
        /// Resumes the current playing track
        /// </summary>
        [Alias("unpause")]
        [Command("resume", RunMode = RunMode.Async)]
        [Summary("Resumes the currently paused track. [Usage] !resume, !unpause")]
        public async Task Resume()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            // Player is in invalid state
            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("The music is already playing!");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync("Resuming music.");
        }

        /// <summary>
        /// Stops the current playing track
        /// </summary>
        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the currently playing track. [Usage] !stop")]
        public async Task Stop()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("The music is already stopped!");
                return;
            }

            await player.StopAsync();
            await ReplyAsync("Stopped music. Current track cannot be replayed.");
        }

        /// <summary>
        /// Disconnects the bot from the connected voice channel
        /// </summary>
        [Command("leave", RunMode = RunMode.Async)]
        [Summary("Disconnects the bot from the connected voice channel. [Usage] !leave")]
        public async Task Leave()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (player.Vueue.Any())
            {
                player.Vueue.Clear();
                await ReplyAsync($"Queue has been cleared!");
            }

            try
            {
                var channel = player.VoiceChannel;
                await _lavaNode.LeaveAsync(channel);
                await ReplyAsync($"Leaving voice channel {channel.Name}.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        /// <summary>
        /// Displays the currently playing track
        /// </summary>
        [Alias("np")]
        [Command("nowplaying", RunMode = RunMode.Async)]
        [Summary("Displays the currently playing track. [Usage] !nowplaying, !np")]
        public async Task NowPlaying()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (player.PlayerState != PlayerState.Paused && player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync($"Nothing is playing currently.");
                return;
            }

            var track = player.Track;
            await ReplyAsync("Now playing:", false, await LavaHelper.GetTrackInfoAsEmbedAsync(track));
        }

        /// <summary>
        /// Clears the queue of all tracks
        /// </summary>
        [Command("clear", RunMode = RunMode.Async)]
        [Summary("Clears the queue of all tracks. [Usage] !clear")]
        public async Task Clear()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (!player.Vueue.Any())
            {
                await ReplyAsync($"Nothing in the queue. :confused:");
                return;
            }

            player.Vueue.Clear();
            await ReplyAsync($"Queue has been cleared!");
        }

        /// <summary>
        /// Shuffles the current queue
        /// </summary>
        [Command("shuffle", RunMode = RunMode.Async)]
        [Summary("Shuffles the current queue. [Usage] !shuffle")]
        public async Task Shuffle()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            if (!player.Vueue.Any())
            {
                await ReplyAsync($"Nothing in the queue. :confused:");
                return;
            }

            player.Vueue.Shuffle();
            await ReplyAsync($"Queue has been shuffled!");
        }

        private async Task<LavaPlayer<LavaTrack>?> GetPlayer()
        {
            var voiceState = Context.User as IVoiceState;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                if (voiceState?.VoiceChannel == null)
                {
                    await ReplyAsync("You must be connected to a voice channel!");
                    return null;
                }

                try
                {
                    player = await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }

            if ((voiceState?.VoiceChannel is null || player?.VoiceChannel is null) || voiceState.VoiceChannel.Name != player.VoiceChannel.Name)
            {
                await ReplyAsync($"You must in the same voice channel as me! Currently, I am in {player?.VoiceChannel.Name}");
                return null;
            }

            return player;
        }

        private bool UserHasPermission(SocketGuildUser user)
        {
            return user.GuildPermissions.Has(GuildPermission.ManageChannels) || user.Roles.Any(x => x.Name == "DJ");
        }
    }
}
