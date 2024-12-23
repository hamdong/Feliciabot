using Discord;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class RandomizerService : IRandomizerService
    {
        private static readonly Random rand = new();

        public int GetRandom(int max, int min = 0)
        {
            lock (rand) // Ensure thread safety
            {
                return rand.Next(min, max);
            }
        }

        public string GetRandomAttachmentWithMessageFromMessage(IMessage message)
        {
            if (message.Attachments.Count != 0)
            {
                int randomAttachment = GetRandom(message.Attachments.Count);
                return message.Content != string.Empty
                    ? $"{message.Content} {message.Attachments.ElementAt(randomAttachment).Url}"
                    : message.Attachments.ElementAt(randomAttachment).Url;
            }

            return message.Content != string.Empty ? message.Content : "Couldn't find a message :confused:";
        }
    }
}
