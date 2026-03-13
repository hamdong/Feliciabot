using Discord;

namespace Feliciabot.services.interfaces
{
    public interface IUserManagementService
    {
        Task AssignTroubleRoleToUserById(IGuildUser user);
    }
}
