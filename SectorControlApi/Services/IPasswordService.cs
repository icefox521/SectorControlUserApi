namespace SectorControlApi.Services
{
    public interface IPasswordService
    {
        byte[] CreateSalt();
        Task<byte[]> HashPasswordAsync(string password, byte[] salt);
        Task<bool> VerifyHashAsync(string password, byte[] salt, byte[] hash);
    }
}
