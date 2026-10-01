using Discord;

namespace Feliciabot.services.interfaces
{
    public interface IRandomizerService
    {
        public int GetRandom(int maxExclusive);
        public int GetRandom(int minInclusive, int maxExclusive);
        public int GetRandomInclusive(int minInclusive, int maxInclusive);
        public string GetRandomAttachmentWithMessageFromMessage(IMessage message);
    }
}
