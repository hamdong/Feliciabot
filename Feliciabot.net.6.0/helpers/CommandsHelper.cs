using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace Feliciabot.net._6._0.helpers
{
    public static class CommandsHelper
    {
        private static readonly Regex emoteRegex = new Regex("^a*:[a-zA-Z0-9_.-]*:[a-zA-Z0-9_.-]*>$");
        private static readonly Random rand = new();

        public static SocketTextChannel? GetSystemChannelFromGuild(SocketGuild guild)
        {
            var channel = GetSystemChannel(guild);
            channel ??= GetChannelByName(guild, "greetings");
            channel ??= GetChannelByName(guild, "general");

            return channel;
        }

        public static SocketTextChannel? GetGeneralChannelFromGuild(SocketGuild guild)
        {
            var channel = GetSystemChannel(guild);
            channel ??= GetChannelByName(guild, "general_felicia");
            channel ??= GetChannelByName(guild, "general");

            return channel;
        }

        public static SocketTextChannel GetSystemChannel(SocketGuild guild)
        {
            return guild.SystemChannel;
        }

        public static SocketTextChannel? GetTestingChannel(SocketGuild guild)
        {
            return GetChannelByName(guild, "betafelicia-testing");
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
        /// <param name="channelName">Name to search for</param>
        /// <returns>The channel object with the specified name</returns>
        public static SocketTextChannel? GetChannelByName(SocketGuild guild, string channelName)
        {
            foreach (SocketTextChannel ch in guild.TextChannels)
            {
                if (ch != null && ((ITextChannel)ch).Name == channelName)
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
            return query != "" && !query.StartsWith('!') && !query.StartsWith('.') && !query.StartsWith("feh") && !query.Contains('@');
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

        /// <summary>
        /// Parse specified message and return it with a random attachment if one exists
        /// </summary>
        /// <param name="message">Message contents to extract</param>
        /// <returns>Message content as string with url to a random attachment</returns>
        public static string ParseMessageWithAttachments(IMessage message)
        {
            if (message.Attachments.Count != 0)
            {
                int randomAttachment = GetRandomNumber(message.Attachments.Count);
                return message.Content != string.Empty ? $"{message.Content} {message.Attachments.ElementAt(randomAttachment).Url}" :
                    message.Attachments.ElementAt(randomAttachment).Url;
            }

            if (message.Content != string.Empty)
            {
                return message.Content;
            }

            return "Couldn't find a message to post :confused:";
        }

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="max">Hard limit on max value, exclusive</param>
        /// <param name="min">Optional hard limit on min value</param>
        /// <returns>Random number from 0 to the specified max (exclusive)(</returns>
        public static int GetRandomNumber(int max, int min = 0)
        {
            lock (rand) // Ensure thread safety
            {
                return rand.Next(min, max);
            }
        }
    }
}
