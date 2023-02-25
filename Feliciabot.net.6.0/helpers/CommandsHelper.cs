using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace Feliciabot.net._6._0.helpers
{
    public static class CommandsHelper
    {
        private const string GREETINGS_CHANNEL_NAME = "greetings";
        private const string GENERAL_CHANNEL_NAME = "general";
        private static readonly Regex emoteRegex = new Regex("^a*:[a-zA-Z0-9_.-]*:[a-zA-Z0-9_.-]*>$");
        public static readonly string MARIANNE_DANCE_LINK = "https://cdn.discordapp.com/emojis/899319530269061161.gif";

        private static Random rand = new();

        /// <summary>
        /// Determines the system channel from a specified guild
        /// </summary>
        /// <param name="guild">Guild to retrieve system channel from</param>
        /// <returns>Channel object representing the system channel</returns>
        public static SocketTextChannel? GetSystemChannelFromGuild(SocketGuild guild)
        {
            var channel = GetSystemChannel(guild);

            //If no system channel exists just try looking for #greetings or #general
            if (channel == null)
            {
                channel = GetChannelByName(guild, GREETINGS_CHANNEL_NAME);
            }

            if (channel == null)
            {
                channel = GetChannelByName(guild, GENERAL_CHANNEL_NAME);
            }

            return channel;
        }

        /// <summary>
        /// Gets the current designated system channel
        /// </summary>
        /// <param name="guild">Guild to check for system channel</param>
        /// <returns>The system channel, or null if it does not exist</returns>
        public static SocketTextChannel GetSystemChannel(SocketGuild guild)
        {
            return guild.SystemChannel;
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
        /// Retrieves the list of dialogue for Feliciabot to post in commands
        /// </summary>
        /// <param name="path">Path to the dialogue list</param>
        /// <returns>List of dialogues as a string array</returns>
        public static string[] GetStringArrayFromFile(string path)
        {
            string[] dialogueList = System.IO.File.ReadAllLines(path);
            return dialogueList;
        }

        /// <summary>
        /// Removes all empty messages from a message list
        /// </summary>
        /// <param name="messageList">List of messages to parse through</param>
        /// <returns>Redefined list without any empty messages</returns>
        public static IMessage[] CullEmptyContentFromMessageList(IMessage[] messageList)
        {
            return Array.FindAll(messageList, HasContent).ToArray();
        }

        /// <summary>
        /// Determines if a message has text content
        /// </summary>
        /// <param name="msg">Message to check</param>
        /// <returns>True, if the message has text content</returns>
        private static bool HasContent(IMessage msg)
        {
            return msg.Content != "";
        }

        /// <summary>
        /// Determines if the passed query won't execute common bot commands for other bot users
        /// </summary>
        /// <param name="query">Query to check for command syntax</param>
        /// <returns>True, if the specified query doesn't contain common bot commands</returns>
        public static bool IsNonCommandQuery(string query)
        {
            return (query != "" &&
                query.IndexOf('!') != 0 &&
                query.IndexOf('.') != 0 &&
                !query.StartsWith("feh") &&
                !query.Contains('@'));
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
        /// Gets a list of messages that match our query parameters, such as matching the query string or being mentioned by a specified user
        /// </summary>
        /// <param name="messagesInChannel">List of messages in the channel context to check</param>
        /// <param name="searchQuery">Query to check for</param>
        /// <param name="mentionedUser">Mentioned user to compare</param>
        /// <returns>List of messages that satisfies the query parameters</returns>
        public static List<IMessage> GetMessagesMatchingQueryParameters(IEnumerable<IMessage> messagesInChannel, string searchQuery, IUser? mentionedUser = null)
        {
            List<IMessage> userMessages = new List<IMessage>();

            foreach (IMessage message in messagesInChannel)
            {
                // Check if we want this message
                if (!IsNonCommandQuery(message.Content) || message.Author.IsBot)
                {
                    continue;
                }

                // Get the messages if they mention a user, otherwise just check if it matches the query parameter
                if (mentionedUser != null)
                {
                    if (message.Author == mentionedUser)
                        userMessages.Add(message);
                }
                else
                {
                    if (message.Content.ToLower().Contains(searchQuery.ToLower()))
                        userMessages.Add(message);
                }
            }

            return userMessages;
        }

        /// <summary>
        /// Determines a random asortment of quotes to post from a specified message list
        /// </summary>
        /// <param name="messagesList">List of messages to parse through</param>
        /// <param name="mentionedUser">Mentioned user who originally said the quotes</param>
        /// <returns>Posts quotes in the message channel</returns>
        public static string GetMessagesToQuote(List<IMessage> messagesList, IUser? mentionedUser = null)
        {
            const int MAX_QUOTES = 3;
            Random randomSeed = new();
            string quotedMessage = "";

            try
            {
                for (int i = 0; i < MAX_QUOTES; i++)
                {
                    int randIndex = randomSeed.Next(messagesList.Count);
                    quotedMessage += messagesList.ElementAt(randIndex) + "\n";
                    messagesList.RemoveAt(randIndex);
                    if (messagesList.Count == 0)
                        break;
                }

                return (mentionedUser != null) ? "Here is what " + mentionedUser.Username + " has said recently:\n" + quotedMessage : quotedMessage;
            }
            catch (ArgumentOutOfRangeException)
            {
                return "Could not find enough valid messages. :confused:";
            }
            finally
            {
                messagesList.Clear();
            }
        }

        /// <summary>
        /// Get contents of specified message and return it with a random attachment if one exists
        /// </summary>
        /// <param name="message">Message contents to extract</param>
        /// <returns>Message content as string with url to a random attachment</returns>
        public static string GetMessageWithRandomAttachment(IMessage message)
        {
            if (message.Content != string.Empty && message.Attachments.Any())
            {
                int randomAttachment = GetRandomNumber(message.Attachments.Count);
                return message.Content + " " + message.Attachments.ElementAt(randomAttachment).Url;
            }
            else if (message.Content == string.Empty && message.Attachments.Any())
            {
                int randomAttachment = GetRandomNumber(message.Attachments.Count);
                return message.Attachments.ElementAt(randomAttachment).Url;
            }
            else if (message.Content != string.Empty && !message.Attachments.Any())
            {
                return message.Content;
            }
            else
            {
                return "Couldn't find a message to post. :confused:";
            }
        }

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="hardMax">Hard limit on max value, exclusive</param>
        /// <param name="hardMin">Optional hard limit on min value</param>
        /// <returns>Random number from 0 to the specified max (non-inclusive)(</returns>
        public static int GetRandomNumber(int hardMax, int hardMin = 0)
        {
            if (rand == null)
                rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(hardMin, hardMax);
        }
    }
}