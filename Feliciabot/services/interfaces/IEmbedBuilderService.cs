using Discord;

namespace Feliciabot.services.interfaces
{
    public interface IEmbedBuilderService
    {
        public Embed GetBotInfoAsEmbed(string botInfo);
    }
}
