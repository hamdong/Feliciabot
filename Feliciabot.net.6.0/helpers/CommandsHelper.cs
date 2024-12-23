using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;

namespace Feliciabot.net._6._0.helpers
{
    public static class CommandsHelper
    {
        private static readonly Regex emoteRegex = new Regex(
            "^a*:[a-zA-Z0-9_.-]*:[a-zA-Z0-9_.-]*>$"
        );

        public static async Task<ITextChannel?> GetSystemChannelFromGuildAsync(IGuild guild)
        {
            var channel = await GetSystemChannelAsync(guild);
            channel ??= await GetChannelByNameAsync(guild, "greetings");
            channel ??= await GetChannelByNameAsync(guild, "general");

            return channel;
        }

        public static async Task<ITextChannel?> GetGeneralChannelFromGuildAsync(IGuild guild)
        {
            var channel = await GetSystemChannelAsync(guild);
            channel ??= await GetChannelByNameAsync(guild, "general_felicia");
            channel ??= await GetChannelByNameAsync(guild, "general");

            return channel;
        }

        public static async Task<ITextChannel?> GetSystemChannelAsync(IGuild guild)
        {
            if (guild.SystemChannelId is null)
                return null;

            var channel = await guild.GetTextChannelAsync((ulong)guild.SystemChannelId);
            return channel;
        }

        public static async Task<IMessageChannel?> GetTestingChannel(SocketGuild guild)
        {
            return await GetChannelByNameAsync(guild, "betafelicia-testing");
        }

        public static DateTime GetCurrentTimeEastern()
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
        }

        /// <summary>
        /// Gets the channel object from the name in the channel list
        /// If more than one channel with the same name exists, return the first occurence in the list
        /// </summary>
        /// <param name="guild">Guild to check for channel</param>
        /// <param name="name">Name to search for</param>
        /// <returns>The channel object with the specified name</returns>
        public static async Task<ITextChannel?> GetChannelByNameAsync(IGuild guild, string name)
        {
            var channels = await guild.GetTextChannelsAsync();
            if (channels == null)
                return null;

            foreach (ITextChannel ch in channels.OfType<ITextChannel>())
            {
                if (ch != null && ch.Name == name)
                {
                    return ch;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if the passed query won't execute common bot commands for other bot users
        /// </summary>
        /// <param name="query">Query to check for command syntax</param>
        /// <returns>True, if the specified query doesn't contain common bot commands</returns>
        public static bool IsNonCommandQuery(string query)
        {
            return query != ""
                && !query.StartsWith('!')
                && !query.StartsWith('.')
                && !query.StartsWith("feh")
                && !query.Contains('@');
        }

        /// <summary>
        /// Determines if the passed message is just an emote
        /// </summary>
        /// <param name="message">message to check</param>
        /// <returns>True, if the passed message is just an emote</returns>
        public static bool IsEmoteMessage(string message)
        {
            return emoteRegex.IsMatch(message);
        }
    }
}
