namespace Feliciabot.net._6._0.services
{
    public class UserManagementService
    {
        private readonly GuildService _guildService;

        public UserManagementService(GuildService guildService)
        {
            _guildService = guildService;
        }

        public async Task AssignTroubleRoleToUserById(ulong guildId, ulong userId)
        {
            var roleId = _guildService.GetRoleIdByName(guildId, "trouble");
            if (roleId == 0) return;
            await _guildService.AddRoleToUserByIdAsync(guildId, userId, roleId);
        }
    }
}
