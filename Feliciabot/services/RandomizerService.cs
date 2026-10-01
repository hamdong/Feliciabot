using Discord;
using Feliciabot.services.interfaces;

namespace Feliciabot.services
{
    public class RandomizerService : IRandomizerService
    {
        public int GetRandom(int maxExclusive) => Random.Shared.Next(maxExclusive);

        public int GetRandom(int minInclusive, int maxExclusive) =>
            Random.Shared.Next(minInclusive, maxExclusive);

        public int GetRandomInclusive(int minInclusive, int maxInclusive) =>
            (int)Random.Shared.NextInt64(minInclusive, (long)maxInclusive + 1);

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
