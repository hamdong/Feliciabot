using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Feliciabot.net._6._0.services
{
    internal class CommandHandler
    {
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LavaNode _lavaNode;
        private readonly IServiceProvider _services;

        private readonly Random randomSeedForDialogues;

        private readonly string[] quoteList;
        private readonly string[] greetingList;

        private readonly string greetingsPath = Environment.CurrentDirectory + @"\data\greetings.txt";
        private readonly string quotesPath = Environment.CurrentDirectory + @"\data\quotes.txt";

        private const string ROLE_TROUBLE = "trouble";

        private readonly string[] MSG_LEWD_REACTION_LIST = { "succ", "lewd" };
        private readonly string[] MSG_BLUSH_REACTION_LIST = { "love", "hug", "kiss" };
        private readonly string[] MSG_HELLO_REACTION_LIST = { "hi", "hello", "yo" };

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, LavaNode lavaNode)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _lavaNode = lavaNode;
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

            // Retrieve any dialogue lists
            greetingList = CommandsHelper.GetStringArrayFromFile(greetingsPath);
            quoteList = CommandsHelper.GetStringArrayFromFile(quotesPath);

            randomSeedForDialogues = new Random();
        }

        public async Task InitializeAsync()
        {
            // client event subscriptions
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += AnnounceJoinedUser;
            _client.UserLeft += AnnounceLeftUser;
            _client.Ready += OnReadyAsync;

            // lavaNode event subscriptions
            _lavaNode.OnTrackEnded += OnTrackEndedAsync;
            _lavaNode.OnTrackStarted += OnTrackStartedAsync;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: _services);

            List<string> commandList = new List<string>();
            foreach (CommandInfo c in _commands.Commands)
            {
                commandList.Add(c.Name + "| " + c.Summary);
            }
            //HelpCommand.commands = commandList;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            SocketUserMessage? message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Return if no channel exists, such as in a DM
            if (message.Channel == null) await Task.CompletedTask;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                foreach (SocketUser u in message.MentionedUsers)
                {
                    // Check if we are pinged by a user who isn't ourselves
                    if (_client.CurrentUser.Username == u.Username &&
                        message.Author.Id != _client.CurrentUser.Id &&
                        !message.Content.StartsWith("!") &&
                        !message.Content.Contains(u.Username))
                    {
                        if (message.Channel is not SocketTextChannel channel)
                        {
                            return;
                        }

                        if (IsWordOccurenceInMsg(MSG_LEWD_REACTION_LIST, message.ToString()))
                        {
                            await channel.SendMessageAsync("L-lewd! :scream:");
                        }
                        else if (IsWordOccurenceInMsg(MSG_BLUSH_REACTION_LIST, message.ToString()))
                        {
                            await channel.SendMessageAsync(":blush:");
                        }
                        else if (IsWordOccurenceInMsg(MSG_HELLO_REACTION_LIST, message.ToString()))
                        {
                            await channel.SendMessageAsync("Hi! N-Nice to see you!");
                        }
                        //Only do a quote if the message starts with Feliciabot's name
                        else if (message.Content.TrimStart('@', '!', '<').StartsWith(u.Mention.TrimStart('@', '!', '<')))
                        {
                            int randIndex = randomSeedForDialogues.Next(quoteList.Length);
                            string quoteToPost = quoteList[randIndex];
                            await channel.SendMessageAsync(quoteToPost);
                        }
                        break;
                    }
                }
            }
            else
            {
                // Create a WebSocket-based command context based on the message
                var context = new SocketCommandContext(_client, message);

                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                await _commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _services);
            }
        }

        /// <summary>
        /// Task to run upon initialization for the connection call
        /// </summary>
        /// <returns>Task representing the connected status</returns>
        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
        }

        /// <summary>
        /// Event on User joining the server
        /// </summary>
        /// <param name="user">User who joined the server</param>
        /// <returns>Nothing, sends greeting message to the system channel</returns>
        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var guild = user.Guild;

            if (guild != null)
            {
                // Find the System (Greetings) channel
                var channel = CommandsHelper.GetSystemChannelFromGuild(guild);

                if(channel is null) return;

                // Get a random welcome message
                int randIndex = randomSeedForDialogues.Next(greetingList.Length);
                string quoteToPost = greetingList[randIndex];

                // Welcome the user
                await channel.SendMessageAsync("Welcome to " + channel.Guild.Name + ", " + user.Mention + "! " + quoteToPost);

                // Assign trouble role if such a role exists
                await AssignRoleToUser(ROLE_TROUBLE, user);
            }
        }

        /// <summary>
        /// Assigns an existing role to a specified user, NOTE: Requires manage roles permission
        /// </summary>
        /// <param name="roleName">Name of existing role to assign to user</param>
        /// <param name="user">User to assign role to</param>
        /// <returns>Nothing, user should have assigned role</returns>
        private async Task AssignRoleToUser(string roleName, SocketGuildUser user)
        {
            foreach (IRole r in user.Guild.Roles)
            {
                if (r.Name == roleName)
                {
                    await user.AddRoleAsync(r);
                    break;
                }
            }
        }

        /// <summary>
        /// Event on User leaving the server
        /// </summary>
        /// <param name="guild">Server the user is leaving from</param>
        /// <param name="user">User who left the server</param>
        /// <returns>Nothing, sends leaving message to the system channel</returns>
        public async Task AnnounceLeftUser(SocketGuild guild, SocketUser user)
        {
            if (guild != null)
            {
                var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
                if (channel is null) return;
                await channel.SendMessageAsync("ok " + user.Username + ".");
            }
        }

        /// <summary>
        /// Checks if a message contains a word in a given list
        /// </summary>
        /// <param name="list">list of words to check for</param>
        /// <param name="message">message to parse through</param>
        /// <returns>True, if the message contains one of the words in the given list</returns>
        private static bool IsWordOccurenceInMsg(string[] list, string message)
        {
            foreach (string s in list)
            {
                if (Regex.IsMatch(message, @"\b" + s + @"\b"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event triggers whenever a track ends, used for checking if the bot should initiate an auto disconnect
        /// </summary>
        /// <param name="args">Arguments containing info on the status of what caused the track end event</param>
        /// <returns>Task containing the message to send back to the caller</returns>
        private async Task OnTrackEndedAsync(TrackEndedEventArgs args)
        {
            var player = args.Player;

            if (args.Reason == TrackEndReason.Replaced)
            {
                return;
            }

            // If the track was stopped, commence auto-disconnect if the queue is empty
            if (args.Reason == TrackEndReason.Stopped)
            {
                if (player.Queue == null || player.Queue.Count == 0)
                {
                    await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks for more music!");
                    _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromMinutes(5));
                }
                return;
            }

            // Clear the track from the queue upon it ending
            if (!player.Queue.TryDequeue(out var queueable))
            {
                await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks for more music!");
                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromMinutes(5));
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);

            var builder = new EmbedBuilder();
            string art = await track.FetchArtworkAsync();
            builder.WithTitle(track.Title);
            builder.AddField("Source", $"[{track.Url}]({track.Url})");
            builder.AddField("Duration", track.Duration.ToString("hh\\:mm\\:ss"), true);
            builder.AddField("Remaining", (track.Duration - track.Position).ToString("hh\\:mm\\:ss"), true);
            builder.WithThumbnailUrl(art);
            builder.WithFooter("\u200B", CommandsHelper.MARIANNE_DANCE_LINK); // unicode string added because empty string won't let footer render

            await args.Player.TextChannel.SendMessageAsync($"{args.Reason}: {args.Track.Title}\nNow playing:", false, builder.Build());
        }

        /// <summary>
        /// Event triggers whenever a track starts
        /// </summary>
        /// <param name="arg">Arguments containing info on the status of what caused the track start event</param>
        /// <returns>Task containing the message to send back to the caller</returns>
        private async Task OnTrackStartedAsync(TrackStartEventArgs arg)
        {
            if (!_disconnectTokens.TryGetValue(arg.Player.VoiceChannel.Id, out var value))
            {
                return;
            }

            if (value.IsCancellationRequested)
            {
                return;
            }

            value.Cancel(true);
            await arg.Player.TextChannel.SendMessageAsync("Auto disconnect has been cancelled!");
        }

        /// <summary>
        /// Call to disconnect the bot from the connect voice channel
        /// </summary>
        /// <param name="player">Music player object to disconnect</param>
        /// <param name="timeSpan">Timespan to specify delay before disconnect</param>
        /// <returns>Task containing the message to send back to the caller</returns>
        private async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
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
        /// <returns>Task containing the message to send back to the caller</returns>
        private async Task OnTrackException(TrackExceptionEventArgs arg)
        {
            await arg.Player.TextChannel.SendMessageAsync($"Track {arg.Track.Title} threw an exception. Exception has been logged to console.");
            LogMessage msg = new LogMessage(LogSeverity.Error, arg.GetType().ToString(), arg.Exception.Message);
            await Main.LogHandler(msg);
            arg.Player.Queue.Enqueue(arg.Track);
            await arg.Player.TextChannel.SendMessageAsync($"{arg.Track.Title} has been re-added to queue after throwing an exception.");
        }

        /// <summary>
        /// Call to log exception if track is stuck
        /// </summary>
        /// <param name="arg">exception argument containing information</param>
        /// <returns>Task containing the message to send back to the caller</returns>
        private async Task OnTrackStuck(TrackStuckEventArgs arg)
        {
            await arg.Player.TextChannel.SendMessageAsync($"Track {arg.Track.Title} got stuck for {arg.Threshold}ms.");
            arg.Player.Queue.Enqueue(arg.Track);
            await arg.Player.TextChannel.SendMessageAsync($"{arg.Track.Title} has been re-added to queue after getting stuck.");
        }

        /// <summary>
        /// Call to log exception if web socket connection is lost
        /// </summary>
        /// <param name="arg">exception argument containing information</param>
        /// <returns>Task containing the message to send back to the caller</returns>
        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            LogMessage msg = new LogMessage(LogSeverity.Error, arg.GetType().ToString(), $"Discord WebSocket connection closed with following reason: {arg.Reason}");
            Main.LogHandler(msg);
            return Task.CompletedTask;
        }
    }
}
