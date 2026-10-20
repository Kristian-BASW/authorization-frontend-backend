using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Security.Services.Security;

public class Argon2PasswordHasher : IPasswordHasher
{
    public string HashAndSaltPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        return HashPassword(password, Convert.ToBase64String(salt));
    }

    public string HashPassword(string password, string salt)
    {
        Argon2id argon2 = new Argon2id(
            Encoding.UTF8.GetBytes(password));

        argon2.Salt = Convert.FromBase64String(salt);
        argon2.MemorySize = 64 * 1024;
        argon2.Iterations = 3;
        argon2.DegreeOfParallelism = 4;

        var hash = argon2.GetBytes(32);
        
        var hashBase = Convert.ToBase64String(hash);

        return $"{hashBase}.{salt}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            throw new InvalidDataException("The password or hashedpassword is not set!");
        }

        string[] parts = hashedPassword.Split('.');
        if (parts.Length != 2)
        {
            throw new InvalidDataException("Can not differentiate the hashed password");
        }

        byte[] storedHash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        if (storedHash.Length != 32 || salt.Length != 16)
        {
            return false;
        }

        string computedPassword = HashPassword(password, parts[1]);
        byte[] computedHash = Convert.FromBase64String(computedPassword.Split('.')[0]);

        return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        
    }
}
