﻿using Discord;

namespace Feliciabot.net._6._0.services.interfaces
{
    public interface IUserManagementService
    {
        Task AssignTroubleRoleToUserById(IGuildUser user);
    }
}
