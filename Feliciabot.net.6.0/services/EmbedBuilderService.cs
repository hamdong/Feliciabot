using Discord;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class EmbedBuilderService(IDiscordClient _client) : IEmbedBuilderService
    {
        public Embed GetBotInfoAsEmbed(string botInfo)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", botInfo);
            builder.WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());
            builder.WithColor(Color.LightGrey);
            return builder.Build();
        }
    }
}
