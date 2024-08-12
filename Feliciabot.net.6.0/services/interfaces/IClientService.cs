using Discord;
using Feliciabot.Abstractions.models;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IClientService
    {
        public string GetUsername();
        public UserStatus GetStatus();
        public IReadOnlyCollection<IActivity> GetActivities();
        public Guild GetGuildById(ulong id);
        public User? GetUserByGuildById(ulong guildId, ulong userId);
        public Channel? GetChannelByGuildById(ulong guildId, ulong channelId);
    }
}
