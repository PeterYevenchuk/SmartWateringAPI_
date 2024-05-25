using System.Security.Cryptography;

namespace SmartWatering.Core.Helpers.PasswordHasher;

public static class GenerateSalt
{
    public static byte[] GenerateRandomSalt(int length)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] saltBytes = new byte[length];
            rng.GetBytes(saltBytes);
            return saltBytes;
        }
    }
}
