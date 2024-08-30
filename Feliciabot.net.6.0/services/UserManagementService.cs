using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class UserManagementService(IGuildService guildService) : IUserManagementService
    {
        public async Task AssignTroubleRoleToUserById(ulong guildId, ulong userId)
        {
            var roleId = guildService.GetRoleIdByName(guildId, "trouble");
            if (roleId == 0) return;
            await guildService.AddRoleToUserByIdAsync(guildId, userId, roleId);
        }
    }
}
