using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class GreetingService : IGreetingService
    {
        private readonly string greetingsPath =
            Environment.CurrentDirectory + @"\data\greetings.txt";
        private readonly string quotesPath = Environment.CurrentDirectory + @"\data\quotes.txt";

        private static readonly (List<string>, string)[] Reactions =
        [
            (["succ", "lewd"], "L-lewd :scream:"),
            (["love", "hug", "kiss"], ":blush:"),
            (["hi", "hello", "yo"], "Hi N-Nice to see you!"),
        ];

        private readonly DiscordSocketClient _client;
        private readonly IUserManagementService _userManagementService;

        private readonly Random randomSeedForDialogues;
        private readonly string[] greetingList;
        private readonly string[] quoteList;

        public GreetingService(
            DiscordSocketClient client,
            IUserManagementService userManagementService
        )
        {
            _client = client;
            _userManagementService = userManagementService;
            quoteList = File.ReadAllLines(quotesPath);
            greetingList = File.ReadAllLines(greetingsPath);
            randomSeedForDialogues = new Random();
        }

        public async Task ReplyToNonCommand(SocketUserMessage message)
        {
            if (!ShouldRespond(message))
                return;
            if (message.Channel is not SocketTextChannel channel)
                return;

            var matchingReaction = Reactions
                .ToList()
                .Find(reaction =>
                    reaction.Item1.Exists(word =>
                        message.Content.Contains(word, StringComparison.OrdinalIgnoreCase)
                    )
                );

            if (matchingReaction.Item2 != null)
            {
                await channel.SendMessageAsync(matchingReaction.Item2);
            }
            else
            {
                int randIndex = randomSeedForDialogues.Next(quoteList.Length);
                string quoteToPost = quoteList[randIndex];
                await channel.SendMessageAsync(quoteToPost);
            }
        }

        public async Task HandleOnUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            if (guild == null)
                return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null)
                return;

            int randIndex = randomSeedForDialogues.Next(greetingList.Length);
            string quoteToPost = greetingList[randIndex];

            await channel.SendMessageAsync(
                $"Welcome to {channel.Guild.Name}, {user.Mention}! {quoteToPost}"
            );

            await _userManagementService.AssignTroubleRoleToUserById(user);
        }

        public async Task HandleOnUserLeft(SocketGuild guild, SocketUser user)
        {
            if (guild == null)
                return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null)
                return;

            await channel.SendMessageAsync($"ok {user.Username}.");
        }

        private bool ShouldRespond(SocketMessage message)
        {
            // message.Reference refers to replying to Discord messsages
            return message.MentionedUsers.Any(su => su.Username == _client.CurrentUser.Username)
                && message.Author.Id != _client.CurrentUser.Id
                && message.Reference is null;
        }
    }
}
