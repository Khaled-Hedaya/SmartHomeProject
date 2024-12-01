using SmartHomeProject.DTOs;
using SmartHomeProject.Models;
using System.Security.Cryptography;

namespace SmartHomeProject.Services
{
    
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = GetHash(password, salt);
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        byte[] hash = GetHash(password, salt);
        for (int i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
                return false;
        }
        return true;
    }

    private static byte[] GetHash(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
        return pbkdf2.GetBytes(HashSize);
    }
}
}
