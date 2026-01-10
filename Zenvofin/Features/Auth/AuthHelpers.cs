using System.Security.Cryptography;
using System.Text;

namespace Zenvofin.Features.Auth;

public static class AuthHelpers
{
    public static string HashToken(string token)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    public static string UserInfoKey(Guid userId) => $"UserInfo:{userId}";
}