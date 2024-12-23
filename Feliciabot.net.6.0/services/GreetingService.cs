using Discord;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using Microsoft.Extensions.Configuration;

namespace Feliciabot.net._6._0.services
{
    public class GreetingService : IGreetingService
    {
        private readonly IDiscordClient _client;
        private readonly IUserManagementService _userManagementService;
        private readonly IRandomizerService _randomizerService;

        private readonly string[] greetingList;
        private readonly string[] quoteList;

        private static readonly (List<string>, string)[] Reactions =
        [
            (["succ", "lewd"], "L-lewd :scream:"),
            (["love", "hug", "kiss"], ":blush:"),
            (["hi", "hello", "yo"], "Hi N-Nice to see you!"),
        ];

        public GreetingService(
            IConfiguration configuration,
            IDiscordClient client,
            IUserManagementService userManagementService,
            IRandomizerService randomizerService
        )
        {
            _client = client;
            _userManagementService = userManagementService;
            _randomizerService = randomizerService;
            greetingList = Responses.GreetingResponses;
            quoteList = File.ReadAllLines(configuration["QuotesPath"]!);
        }

        public async Task ReplyToNonCommand(IUserMessage message)
        {
            if (!ShouldRespond(message))
                return;

            if (message.Channel is not IMessageChannel channel)
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
                return;
            }

            int randIndex = _randomizerService.GetRandom(quoteList.Length);
            string quoteToPost = quoteList[randIndex];
            await channel.SendMessageAsync(quoteToPost);
        }

        public async Task HandleOnUserJoined(IGuildUser user)
        {
            var guild = user.Guild;
            if (guild == null)
                return;

            var channel = await CommandsHelper.GetSystemChannelFromGuildAsync(guild);
            if (channel is null)
                return;

            int randIndex = _randomizerService.GetRandom(greetingList.Length);
            string quoteToPost = greetingList[randIndex];

            await channel.SendMessageAsync(
                $"Welcome to {channel.Guild.Name}, {user.Mention}! {quoteToPost}"
            );

            await _userManagementService.AssignTroubleRoleToUserById(user);
        }

        public async Task HandleOnUserLeft(IGuild guild, IUser user)
        {
            if (guild == null)
                return;

            var channel = await CommandsHelper.GetSystemChannelFromGuildAsync(guild);
            if (channel is null)
                return;

            await channel.SendMessageAsync($"ok {user.Username}.");
        }

        private bool ShouldRespond(IUserMessage message)
        {
            // message.Reference refers to replying to Discord messsages
            return message.MentionedUserIds.Any(id => id == _client.CurrentUser.Id)
                && message.Author.Id != _client.CurrentUser.Id
                && message.Reference is null;
        }
    }
}
