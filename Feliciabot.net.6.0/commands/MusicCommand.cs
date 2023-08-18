using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria.Responses.Search;
using Feliciabot.net._6._0.helpers;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using Victoria.Node;
using Victoria.Player;
using Victoria;

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
            if (_lavaNode.IsConnected)
                await ReplyAsync("Music player is connected! :smiley:");
            else
                await ReplyAsync("Music player is not connected! :scream:");
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
                if (getPlayerSuccess)
                {
                    await ReplyAsync($"I've already joined in voice channel {player.VoiceChannel}.");
                }
                else
                {
                    await ReplyAsync($"Unable to get player :(!");
                }
                return;
            }

            var voiceState = Context.User as IVoiceState;

            if (voiceState is null)
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
                await GetTrackInfoAsEmbed("Now playing:", player.Track);
                return;
            }

            if (!player.Vueue.Any())
            {
                await ReplyAsync("Play what? There are no more songs in the queue!");
                return;
            }

            var nextTrack = await player.SkipAsync();
            await GetTrackInfoAsEmbed("Now playing:", nextTrack.Current);
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
            await GetTrackInfoAsEmbed("Now playing:", lavaTrack);
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
                await GetTrackInfoAsEmbed($"Skipped: {skipped.Title}\nNow playing: {current.Title}", current);
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

            /*TODO: Might not need this?
            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Cannot pause stopped music. :confused:");
                return;
            }*/

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
        /// Reconnect to the bot to vc, retaining the current playlist if there is one
        /// </summary>
        [Alias("rc")]
        [Command("reconnect", RunMode = RunMode.Async)]
        [Summary("Reconnects bot to voice channel and retains the current queue. [Usage] !reconnect, rc")]
        public async Task Reconnect()
        {
            LavaPlayer<LavaTrack>? player = await GetPlayer();
            if (player == null) return;

            List<LavaTrack> currentQueue = new();
            LavaTrack? currentTrack = null;
            var channel = player.VoiceChannel;

            // Get the current track if there is one
            if (player.Track != null)
            {
                currentTrack = player.Track;
                await ReplyAsync($"Current track has been stored!");
            }

            // Get all tracks in the queue
            if (player.Vueue.Any())
            {
                currentQueue = player.Vueue.ToList();
                await ReplyAsync($"Current queue has been stored!");
            }

            // Disconnect from voice channel
            try
            {
                await player.StopAsync();
                await _lavaNode.LeaveAsync(channel);
                await ReplyAsync($"Leaving voice channel {channel.Name}.");
            }
            catch (InvalidOperationException e)
            {
                await ReplyAsync($"Command failed with the following error message: {e.Message}. Try running again.");
                return;
            }

            // Rejoin voice channel
            await _lavaNode.JoinAsync(channel, Context.Channel as ITextChannel);
            await ReplyAsync($"Reconnected to {channel.Name}!");

            // Get the new player
            LavaPlayer<LavaTrack>? newPlayer = await GetPlayer();
            if (newPlayer == null) return;

            // Play the current track upon reconnect and seek to previous position
            if (currentTrack != null)
            {
                await newPlayer.PlayAsync(currentTrack);
                await newPlayer.SeekAsync(currentTrack.Position);
                await GetTrackInfoAsEmbed("Now playing:", currentTrack);
            }
            else
            {
                await ReplyAsync($"Current track was lost. :sob:");
            }

            // Queue up the remaining songs in the current queue
            if (currentQueue.Any())
            {
                newPlayer.Vueue.Enqueue(currentQueue);
                await ReplyAsync($"Enqueued {currentQueue.Count} tracks.");

                if (player.PlayerState != PlayerState.Playing)
                {
                    newPlayer.Vueue.TryDequeue(out var lavaTrack);
                    await newPlayer.PlayAsync(lavaTrack);
                    await GetTrackInfoAsEmbed("Skipped to next song. Now playing:", newPlayer.Track);
                }
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

            await GetTrackInfoAsEmbed("Now playing:", player.Track);
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

            if (voiceState?.VoiceChannel.Name != player?.VoiceChannel.Name)
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

        /// <summary>
        /// Responds with an embeded message containing the specified track's information
        /// </summary>
        /// <param name="message">Message to go with the embed</param>
        /// <param name="track">Track to display information on</param>
        /// <returns>Embedded message containing track information</returns>
        private async Task GetTrackInfoAsEmbed(string message, LavaTrack track)
        {
            var builder = new EmbedBuilder();
            string art = await track.FetchArtworkAsync();


            builder.WithAuthor(track.Author, CommandsHelper.MARIANNE_DANCE_LINK, track.Url);
            builder.WithTitle(track.Title);
            builder.WithUrl($"{track.Url}");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.AddField("Remaining", (track.Duration - track.Position).ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(art);
            await ReplyAsync(message, false, builder.Build());
        }
    }
}
