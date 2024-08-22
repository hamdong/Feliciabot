using Feliciabot.Abstractions.models;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IGuildService
    { 
        public Channel? GetChannelByGuildById(ulong guildId, ulong channelId);
        public Task AddRoleToUserByIdAsync(ulong guildId, ulong userId, ulong roleId);
        public ulong GetRoleIdByName(ulong guildId, string name);
    }
}
