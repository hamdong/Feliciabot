using Discord.WebSocket;

namespace Feliciabot.net._6._0.services
{
    public class UserManagementService
    {
        protected UserManagementService() { }

        public static async Task AssignTroubleRoleToUser(SocketGuildUser user)
        {
            var serverRole = user.Guild.Roles.FirstOrDefault(role => role.Name == "trouble");
            if (serverRole == null) return;
            await user.AddRoleAsync(serverRole);
        }
    }
}
