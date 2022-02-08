using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace SectorControlApi.Services
{
    /// <summary>
    /// Provides basic cryptographic functions for passwords.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        public byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;
        }

        public async Task<byte[]> HashPasswordAsync(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 64 * 64;
            return await argon2.GetBytesAsync(32);
        }

        public async Task<bool> VerifyHashAsync(string password, byte[] salt, byte[] hash)
        {
            var newHash = await HashPasswordAsync(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
