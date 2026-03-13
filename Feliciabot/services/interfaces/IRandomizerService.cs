using Discord;

namespace Feliciabot.services.interfaces
{
    public interface IRandomizerService
    {
        public int GetRandom(int max, int min = 0);
        public string GetRandomAttachmentWithMessageFromMessage(IMessage message);
    }
}
