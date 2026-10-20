namespace Security.Services.Security;

public interface IPasswordHasher
{
    string HashAndSaltPassword(string password);
    string HashPassword(string password, string salt);
    bool VerifyPassword(string password, string hashedPassword);
}
