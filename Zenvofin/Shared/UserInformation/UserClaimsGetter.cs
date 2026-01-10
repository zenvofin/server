using System.Security.Claims;

namespace Zenvofin.Shared.UserInformation;

public static class UserClaimsGetter
{
    public static bool TryGetClaims(this ClaimsPrincipal user, out UserClaims claims)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        string? deviceId = user.FindFirstValue(ClaimTypes.Thumbprint);

        claims = new UserClaims();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(deviceId))
            return false;

        if (!Guid.TryParse(userId, out Guid userGuid) ||
            !Guid.TryParse(deviceId, out Guid deviceGuid))
            return false;

        claims = new UserClaims { UserId = userGuid, DeviceId = deviceGuid };
        return true;
    }
}