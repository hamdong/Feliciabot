﻿using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.RegularExpressions;
using Victoria.Node;

namespace Feliciabot.net._6._0.services
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
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
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;

            // Retrieve any dialogue lists
            greetingList = CommandsHelper.GetStringArrayFromFile(greetingsPath);
            quoteList = CommandsHelper.GetStringArrayFromFile(quotesPath);

            randomSeedForDialogues = new Random();
        }

        public async Task InitializeAsync()
        {
            _client.Ready += OnReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += AnnounceJoinedUser;
            _client.UserLeft += AnnounceLeftUser;
            _services.GetRequiredService<AudioService>();

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

            List<string> commandList = new();
            foreach (CommandInfo c in _commands.Commands)
            {
                commandList.Add(c.Name + "| " + c.Summary);
            }
            //HelpCommand.commands = commandList;
        }

        private async Task OnReadyAsync()
        {
            var node = _services.GetRequiredService<LavaNode>();
            if (!node.IsConnected)
            {
                await node.ConnectAsync();
            }

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
                        !message.Content.StartsWith('!') &&
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
        /// Event on User joining the server
        /// </summary>
        /// <param name="user">User who joined the server</param>
        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var guild = user.Guild;
            if (guild == null) return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null) return;

            // Get a random welcome message
            int randIndex = randomSeedForDialogues.Next(greetingList.Length);
            string quoteToPost = greetingList[randIndex];

            await channel.SendMessageAsync("Welcome to " + channel.Guild.Name + ", " + user.Mention + "! " + quoteToPost);
            await AssignRoleToUser(ROLE_TROUBLE, user);
        }

        /// <summary>
        /// Event on User leaving the server
        /// </summary>
        /// <param name="guild">Server the user is leaving from</param>
        /// <param name="user">User who left the server</param>
        public async Task AnnounceLeftUser(SocketGuild guild, SocketUser user)
        {
            if (guild == null) return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null) return;
            await channel.SendMessageAsync("ok " + user.Username + ".");
        }

        private static async Task AssignRoleToUser(string roleName, SocketGuildUser user)
        {
            var serverRole = user.Guild.Roles.FirstOrDefault(role => role.Name == roleName);
            if (serverRole == null) return;
            await user.AddRoleAsync(serverRole);
        }

        private static bool IsWordOccurenceInMsg(string[] checkWordList, string message)
        {
            var escapedWords = checkWordList.Select(w => @"\b" + Regex.Escape(w) + @"\b");
            bool containsWord = escapedWords.Any(pattern => Regex.IsMatch(message, pattern));
            return containsWord;
        }
    }
}
