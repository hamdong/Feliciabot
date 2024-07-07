using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.services
{
    public class GreetingService
    {
        private readonly string greetingsPath = Environment.CurrentDirectory + @"\data\greetings.txt";
        private readonly string quotesPath = Environment.CurrentDirectory + @"\data\quotes.txt";

        private static readonly (List<string>, string)[] Reactions = [
            (new List<string> { "succ", "lewd" }, "L-lewd :scream:"),
            (new List<string> { "love", "hug", "kiss" }, ":blush:"),
            (new List<string> { "hi", "hello", "yo" }, "Hi N-Nice to see you!")
        ];

        private readonly DiscordSocketClient _client;
        private readonly UserManagementService _userManagementService;

        private readonly Random randomSeedForDialogues;
        private readonly string[] greetingList;
        private readonly string[] quoteList;

        public GreetingService(DiscordSocketClient client, UserManagementService userManagementService)
        {
            _client = client;
            _userManagementService = userManagementService;
            quoteList = File.ReadAllLines(quotesPath);
            greetingList = File.ReadAllLines(greetingsPath);
            randomSeedForDialogues = new Random();
        }

        public async Task ReplyToNonCommand(SocketUserMessage message)
        {
            if (!ShouldRespond(message)) return;
            if (message.Channel is not SocketTextChannel channel) return;

            bool reacted = false;
            var msgText = message.Content;

            foreach (var reaction in Reactions)
            {
                if (reaction.Item1.Exists(reaction => reaction.Any(word => msgText.Contains(word))))
                {
                    await channel.SendMessageAsync(reaction.Item2);
                    reacted = true;
                    break;
                }
            }

            if (!reacted)
            {
                int randIndex = randomSeedForDialogues.Next(quoteList.Length);
                string quoteToPost = quoteList[randIndex];
                await channel.SendMessageAsync(quoteToPost);
            }
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var guild = user.Guild;
            if (guild == null) return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null) return;

            int randIndex = randomSeedForDialogues.Next(greetingList.Length);
            string quoteToPost = greetingList[randIndex];

            await channel.SendMessageAsync($"Welcome to {channel.Guild.Name}, {user.Mention}! {quoteToPost}");
            await _userManagementService.AssignTroubleRoleToUserById(guild.Id, user.Id);
        }

        public static async Task AnnounceLeftUser(SocketGuild guild, SocketUser user)
        {
            if (guild == null) return;

            var channel = CommandsHelper.GetSystemChannelFromGuild(guild);
            if (channel is null) return;

            await channel.SendMessageAsync($"ok {user.Username}.");
        }

        private bool ShouldRespond(SocketMessage message)
        {
            return message.MentionedUsers.Any(su => su.Username == _client.CurrentUser.Username)
                && message.Author.Id != _client.CurrentUser.Id;
        }
    }
}
