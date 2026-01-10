using Microsoft.AspNetCore.Identity;

namespace Zenvofin.Features.Auth.Data;

public sealed class User : IdentityUser<Guid>
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}