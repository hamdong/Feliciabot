namespace Feliciabot.Abstractions.interfaces
{
    public interface IUser
    {
        public Task AddRoleByIdAsync(ulong roleId);
    }
}
