using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class GuildService(IClientService clientService) : IGuildService
    {
        public virtual async Task AddRoleToUserByIdAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var user = clientService.GetUserByGuildById(guildId, userId);
            if (user == null) return;
            await user.AddRoleByIdAsync(roleId);
        }

        public virtual ulong GetRoleIdByName(ulong guildId, string name)
        {
            var role = clientService.GetGuildById(guildId).Roles.ToList().Find(r => r.Name == name);
            return role?.Id ?? default; // 0 if not found
        }

        public virtual IReadOnlyCollection<GuildEmote> GetEmotesFromGuild(ICommandContext context)
        {
            return context.Guild.Emotes;
        }
    }
}
