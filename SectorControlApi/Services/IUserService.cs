using SectorControlApi.Data.Users;

namespace SectorControlApi.Services
{
    public interface IUserService
    {
        Task CreateUserInDatabaseAsync(string username, string email, string salt, string passwordHash);
        Task<User> RetrieveUserLoginDetailAsync(string email);
        Task<Guid?> AuthenticateAsync(string email, string password);
        Task<int?> VerifyTokenAsync(string token);
    }
}
