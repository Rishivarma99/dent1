using Dent1.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dent1.Business.Security;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}