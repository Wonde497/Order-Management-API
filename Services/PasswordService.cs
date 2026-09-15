using Microsoft.AspNetCore.Identity;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services;

public interface IPasswordService
{
    string HashPassword(string plainTextPassword);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}

public class PasswordService : IPasswordService
{
    // PasswordHasher<T> is generic over the entity type it's hashing for,
    // but doesn't actually use the entity's data - it's just a required type parameter.
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(string plainTextPassword)
    {
        // The "user" argument isn't used by the algorithm itself here -
        // pass null! since we're only hashing, not tying the hash to a specific instance.
        return _hasher.HashPassword(null!, plainTextPassword);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
