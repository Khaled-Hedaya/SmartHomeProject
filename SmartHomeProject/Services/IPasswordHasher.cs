
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    }
}