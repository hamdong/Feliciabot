using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;
using Feliciabot.net._6._0.helpers;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Feliciabot.net._6._0.commands
{
    public class MusicCommand : ModuleBase
    {
        private readonly LavaNode _lavaNode;
        private const int NUM_TRACKS_PER_PAGE = 10;
        private const string MARIANNE_DANCE_EMOTE = "<a:MarianneDance:899319530269061161>";
        private readonly InteractiveService _interactiveService;

        /// <summary>
        /// Constructor for Music command, dependency inject clients
        /// </summary>
        /// <param name="client">client for the connected account</param>
        /// <param name="lavaNode">client for the lavaNode api</param>
        public MusicCommand(LavaNode lavaNode, InteractiveService interactiveService)
        {
            _lavaNode = lavaNode;
            _interactiveService = interactiveService;
        }

        /// <summary>
        /// Checks the status of the music player connection
        /// </summary>
        /// <returns>Task representing the status of the music player</returns>
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
        /// Checks the state of the music player
        /// </summary>
        /// <returns>Task representing the state of the music player</returns>
        [Command("musicplayerstate", RunMode = RunMode.Async)]
        [Summary("Checks the state of the music player . [Usage] !musicplayerstate")]
        public async Task MusicPlayerStat()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            await ReplyAsync($"Music player state is: {player.PlayerState}!");
        }

        /// <summary>
        /// Joins the context call channel
        /// </summary>
        /// <returns>Task containing the status of the join request</returns>
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the context call channel. [Usage] !join")]
        public async Task JoinAsync()
        {
            if (!LavaHelper.IsUserInVoiceChannel(Context.User))
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (IsPlayerConnected())
            {
                await ReplyAsync($"I've already joined in voice channel {_lavaNode.GetPlayer(Context.Guild).VoiceChannel}.");
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

        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Plays a track from a specified song via search query in the current call. [Usage] !play")]
        public async Task Play()
        {
            if (!LavaHelper.IsUserInVoiceChannel(Context.User))
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            // Join voice channel if not already joined
            if (!IsPlayerConnected())
            {
                var voiceState = Context.User as IVoiceState;

                if(voiceState is null)
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
                    return;
                }
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

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

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("Play what? There are no more songs in the queue!");
                return;
            }

            var nextTrack = await player.SkipAsync();
            await GetTrackInfoAsEmbed("Now playing:", nextTrack.Current);
        }

        /// <summary>
        /// Plays the specified track or playlist, and also queues up music to be played
        /// </summary>
        /// <param name="query">Parameter to search for music based on title</param>
        /// <returns>Task representing the play status of the player, plays music in the voice chat</returns>
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Plays a track from a specified song via search query in the current call. [Usage] !play [query]")]
        public async Task Play([Remainder] string query)
        {
            if (!LavaHelper.IsUserInVoiceChannel(Context.User))
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            // Join voice channel if not already joined
            if (!IsPlayerConnected())
            {
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
                    return;
                }
            }

            // Search for music via Youtube or direct Uri
            var searchResponse = Uri.IsWellFormedUriString(query, UriKind.RelativeOrAbsolute) ?
                await _lavaNode.SearchAsync(SearchType.Direct, query) : await _lavaNode.SearchYouTubeAsync(query);

            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            // Check for a playlist and queue up music, otherwise play the searched song and queue anything that's waiting to be played
            if (LavaHelper.IsSearchResponseAPlaylist(searchResponse))
            {
                foreach (var track in searchResponse.Tracks)
                {
                    if (LavaHelper.IsPlayerInUse(player))
                    {
                        player.Queue.Enqueue(track);
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        await GetTrackInfoAsEmbed("Now playing:", track);
                    }
                }
                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
            }
            else
            {
                var track = searchResponse.Tracks.ElementAt(0);
                if (LavaHelper.IsPlayerInUse(player))
                {
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
                else
                {
                    await player.PlayAsync(track);
                    await GetTrackInfoAsEmbed("Now playing:", track);
                }
            }
        }

        /// <summary>
        /// Displays the current remaining items in the queue
        /// </summary>
        /// <returns>Task containing the response for the track queue</returns>
        [Command("queue", RunMode = RunMode.Async)]
        [Alias("playlist", "q")]
        [Summary("Displays the current remaining items in the queue. [Usage] !queue, !playlist, !q")]
        public async Task Queue()
        {

            // Check if user is allowed to play music
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User, true);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Check if there are songs in the queue
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.Queue.Count == 0)
            {
                await ReplyAsync("There are no more songs in the queue!");
                return;
            }

            List<string> trackList = new List<string>();
            TimeSpan totalDuration = new TimeSpan();
            string pageContent = string.Empty;
            int trackNum = 1;
            int trackOnPageCount = 1;
            foreach (LavaTrack track in player.Queue)
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
        /// <returns>Task containing the response from removing from the queue</returns>
        [Alias("rm")]
        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Removes numbered track from the queue. [Usage] !remove [track number], !rm")]
        public async Task Remove(int trackNum)
        {
            // Check if the user is able to play music
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Ensure the queue isn't empty
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.Queue.Count == 0)
            {
                await ReplyAsync("There are no songs in the queue to remove!");
                return;
            }

            // Ensure valid track number
            if (trackNum < 1)
            {
                await ReplyAsync("Invalid track number, value must be greater than 0");
                return;
            }
            else if (trackNum > player.Queue.Count)
            {
                await ReplyAsync($"Invalid track number, value cannot be greater than the total number of tracks currently in the queue: ({player.Queue.Count})");
                return;
            }

            // Get the track from the queue
            int trackCount = 1;
            LavaTrack? trackToRemove = null;
            foreach (LavaTrack track in player.Queue)
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
                player.Queue.Remove(trackToRemove);
                await ReplyAsync($"Removed track: '{trackCount}. {trackToRemove.Title}'");
            }
            else
            {
                await ReplyAsync($"Unable to remove track number ({trackNum})");
            }
        }

        [Alias("fs")]
        [Command("skip", RunMode = RunMode.Async)]
        [Summary("Skips current track and plays the next one in the queue. [Usage] !skip, !fs")]
        public async Task Skip()
        {
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            // Player is in invalid state
            if (player.PlayerState == PlayerState.None)
            {
                await ReplyAsync("I can't perform that operation without a song. :confused:");
                return;
            }

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("Cannot skip when there are no more songs in the queue!");
                return;
            }

            var (_, Current) = await player.SkipAsync();
            await GetTrackInfoAsEmbed("Skipped! Now playing:", Current);
        }

        /// <summary>
        /// Pause the current playing track
        /// </summary>
        /// <returns>Task containing the response from pausing the track</returns>
        [Command("pause", RunMode = RunMode.Async)]
        [Summary("Pauses the currently playing track. [Usage] !pause")]
        public async Task Pause()
        {
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

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

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Cannot pause stopped music. :confused:");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Paused music.");
        }

        /// <summary>
        /// Resumes the current playing track
        /// </summary>
        /// <returns>Task containing the response from resuming the track</returns>
        [Alias("unpause")]
        [Command("resume", RunMode = RunMode.Async)]
        [Summary("Resumes the currently paused track. [Usage] !resume, !unpause")]
        public async Task Resume()
        {
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

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

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Cannot resume stopped music. :confused:");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync("Resuming music.");
        }

        /// <summary>
        /// Stops the current playing track
        /// </summary>
        /// <returns>Task containing the response from stopping the track</returns>
        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the currently playing track. [Usage] !stop")]
        public async Task Stop()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Stop the music if it isn't already stopped
            var player = _lavaNode.GetPlayer(Context.Guild);

            // Player is in invalid state
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
            await ReplyAsync("Stopped music.");
        }

        /// <summary>
        /// Disconnects the bot from the connected voice channel
        /// </summary>
        /// <returns>Task containing the response from leaving the voice channel</returns>
        [Command("leave", RunMode = RunMode.Async)]
        [Summary("Disconnects the bot from the connected voice channel. [Usage] !leave")]
        public async Task Leave()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Stop any music that is playing
            var player = _lavaNode.GetPlayer(Context.Guild);
            var channel = player.VoiceChannel;
            await player.StopAsync();

            // Remove all tracks from the queue
            if (player.Queue.Count > 0)
            {
                player.Queue.Clear();
                await ReplyAsync($"Queue has been cleared!");
            }

            // Disconnect from voice channel
            await _lavaNode.LeaveAsync(channel);
            await ReplyAsync($"Leaving voice channel {channel.Name}.");
        }

        /// <summary>
        /// Reconnect to the bot to vc, retaining the current playlist if there is one
        /// </summary>
        /// <returns>Task containing the response for reconnecting to the voice channel</returns>
        [Alias("rc")]
        [Command("reconnect", RunMode = RunMode.Async)]
        [Summary("Reconnects bot to voice channel and retains the current queue. [Usage] !reconnect, rc")]
        public async Task Reconnect()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            List<LavaTrack> currentQueue = new List<LavaTrack>();
            LavaTrack? currentTrack = null;

            // Stop any music that is playing
            var player = _lavaNode.GetPlayer(Context.Guild);
            var channel = player.VoiceChannel;

            // Get the current track if there is one
            if (player.Track != null)
            {
                currentTrack = player.Track;
                await ReplyAsync($"Current track has been stored!");
            }

            // Get all tracks in the queue
            if (player.Queue.Count > 0)
            {
                currentQueue = player.Queue.ToList();
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
            player = _lavaNode.GetPlayer(Context.Guild);

            // Play the current track upon reconnect and seek to previous position
            if (currentTrack != null)
            {
                await player.PlayAsync(currentTrack);
                await player.SeekAsync(currentTrack.Position);
                await GetTrackInfoAsEmbed("Now playing:", currentTrack);
            }
            else
            {
                await ReplyAsync($"Current track was lost. :sob:");
            }

            // Queue up the remaining songs in the current queue
            if (currentQueue.Count != 0)
            {
                player.Queue.Enqueue(currentQueue);
                await ReplyAsync($"Enqueued {currentQueue.Count + 1} tracks.");

                // Play the next song in the queue if nothing is playing
                if (player.PlayerState != PlayerState.Playing)
                {
                    await player.PlayAsync(player.Queue.ElementAt(0));
                    await GetTrackInfoAsEmbed("Skipped to next song. Now playing:", player.Track);
                }
            }
        }

        /// <summary>
        /// Displays the currently playing track
        /// </summary>
        /// <returns>Task containing the response from displaying the current song</returns>
        [Alias("np")]
        [Command("nowplaying", RunMode = RunMode.Async)]
        [Summary("Displays the currently playing track. [Usage] !nowplaying, !np")]
        public async Task NowPlaying()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User, true);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Show the currently playing song so long as there is a song playing
            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Playing)
            {
                await GetTrackInfoAsEmbed("Now playing:", player.Track);
            }
            else
            {
                await ReplyAsync($"Nothing is playing currently.");
            }
        }

        /// <summary>
        /// Clears the queue of all tracks
        /// </summary>
        /// <returns>Task containing the response from clearing the queue</returns>
        [Command("clear", RunMode = RunMode.Async)]
        [Summary("Clears the queue of all tracks. [Usage] !clear")]
        public async Task Clear()
        {
            // Check if the user is allowed to perform this action
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Clear the tracks from the queue
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.Queue.Count > 0)
            {
                player.Queue.Clear();
                await ReplyAsync($"Queue has been cleared!");
            }
            else
            {
                await ReplyAsync($"Nothing in the queue. :confused:");
            }
        }

        /// <summary>
        /// Shuffle the current queue
        /// </summary>
        /// <returns>Reponse with the queue shuffled</returns>
        [Command("shuffle", RunMode = RunMode.Async)]
        [Summary("Shuffle the current queue. [Usage] !shuffle")]
        public async Task Shuffle()
        {
            // Check if the user can play music
            string playMusicResponse = CanPlayMusic(Context.Guild, Context.User);
            if (!string.IsNullOrEmpty(playMusicResponse))
            {
                await ReplyAsync(playMusicResponse);
                return;
            }

            // Shuffle queue if there are tracks
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.Queue.Count > 0)
            {
                player.Queue.Shuffle();
                await ReplyAsync($"Queue has been shuffled!");
            }
            else
            {
                await ReplyAsync($"Nothing in the queue. :confused:");
            }
        }

        /// <summary>
        /// Check if the music player is connected
        /// </summary>
        /// <returns>True, if the music player is connected</returns>
        private bool IsPlayerConnected()
        {
            return _lavaNode.HasPlayer(Context.Guild);
        }

        /// <summary>
        /// General parameters to check in order for the user to perform music actions
        /// </summary>
        /// <param name="guild">Guild context for the music player</param>
        /// <param name="user">User making the request</param>
        /// <param name="checkingQueue">Ignore DJ role if checking the queue</param>
        /// <returns>Text response containing details on why the user cannot perform an action</returns>
        private string CanPlayMusic(IGuild guild, IUser user, bool checkingQueue = false)
        {
            var voiceState = user as IVoiceState;

            // User isn't connected to voice channel
            if (voiceState?.VoiceChannel == null)
            {
                return "You must be connected to a voice channel!";
            }

            // Bot isn't connected to voice channel
            if (!_lavaNode.HasPlayer(guild))
            {
                return "I'm not connected to a voice channel!";
            }

            var player = _lavaNode.GetPlayer(guild);

            // Bot is in different voice channel from user
            if (voiceState.VoiceChannel.Name != player.VoiceChannel.Name)
            {
                return $"You need to be in the same voice channel as me! Currently, I am in {player.VoiceChannel.Name}";
            }

            // User doesn't have the correct persmissions
            SocketGuildUser socketUser = (SocketGuildUser)user;
            if (!socketUser.GuildPermissions.Has(GuildPermission.ManageChannels) && !socketUser.Roles.Any(x => x.Name == "DJ") && !checkingQueue)
            {
                return "You need either manage permissions access or the DJ role to use this command.";
            }

            return string.Empty;
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

            builder.WithTitle(track.Title);
            builder.AddField("Source", $"[{track.Url}]({track.Url})");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.AddField("Remaining", (track.Duration - track.Position).ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(art);
            builder.WithFooter("\u200B", CommandsHelper.MARIANNE_DANCE_LINK); // unicode string added because empty string won't let footer render
            await ReplyAsync(message, false, builder.Build());
        }
    }
}
