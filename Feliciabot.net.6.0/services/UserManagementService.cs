using Discord;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class UserManagementService() : IUserManagementService
    {
        public async Task AssignTroubleRoleToUserById(IGuildUser user)
        {
            var role = user.Guild.Roles.FirstOrDefault(r => r.Name == "trouble");
            if (role == null)
                return;

            var rollExists = user.RoleIds.Any(r => r == role.Id);
            if (rollExists)
                return;

            await user.AddRoleAsync(role.Id);
        }
    }
}
