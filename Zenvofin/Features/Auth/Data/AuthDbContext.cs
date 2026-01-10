using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Zenvofin.Features.Auth.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.UserName).HasColumnName("name");
            b.Property(u => u.NormalizedUserName).HasColumnName("normalized_name");
            b.Property(u => u.Email).HasColumnName("email");
            b.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
            b.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
            b.Property(u => u.PasswordHash).HasColumnName("password_hash");
            b.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            b.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            b.Property(u => u.PhoneNumber).HasColumnName("phone_number");
            b.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            b.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            b.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            b.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
            b.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
        });

        builder.Entity<IdentityUserToken<Guid>>(b =>
        {
            b.ToTable("user_tokens");
            b.Property(ut => ut.UserId).HasColumnName("user_id");
            b.Property(ut => ut.LoginProvider).HasColumnName("login_provider");
            b.Property(ut => ut.Name).HasColumnName("name");
            b.Property(ut => ut.Value).HasColumnName("value");
        });

        builder.Entity<IdentityRole<Guid>>(b =>
        {
            b.ToTable("roles");
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Name).HasColumnName("name");
            b.Property(r => r.NormalizedName).HasColumnName("normalized_name");
            b.Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");
        });

        builder.Entity<IdentityUserRole<Guid>>(b =>
        {
            b.ToTable("user_roles");
            b.Property(ur => ur.UserId).HasColumnName("user_id");
            b.Property(ur => ur.RoleId).HasColumnName("role_id");
        });

        builder.Entity<IdentityUserLogin<Guid>>(b =>
        {
            b.ToTable("user_logins");
            b.Property(ul => ul.LoginProvider).HasColumnName("login_provider");
            b.Property(ul => ul.ProviderKey).HasColumnName("provider_key");
            b.Property(ul => ul.ProviderDisplayName).HasColumnName("provider_display_name");
            b.Property(ul => ul.UserId).HasColumnName("user_id");
        });

        builder.Entity<IdentityUserClaim<Guid>>(b =>
        {
            b.ToTable("user_claims");
            b.Property(uc => uc.Id).HasColumnName("id");
            b.Property(uc => uc.UserId).HasColumnName("user_id");
            b.Property(uc => uc.ClaimType).HasColumnName("type");
            b.Property(uc => uc.ClaimValue).HasColumnName("value");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(b =>
        {
            b.ToTable("role_claims");
            b.Property(rc => rc.Id).HasColumnName("id");
            b.Property(rc => rc.RoleId).HasColumnName("role_id");
            b.Property(rc => rc.ClaimType).HasColumnName("type");
            b.Property(rc => rc.ClaimValue).HasColumnName("value");
        });

        builder.Entity<RefreshToken>(b =>
        {
            b.HasKey(rt => rt.Id).HasName("pk_refresh_tokens");

            b.ToTable("refresh_tokens");

            b.HasIndex(rt => new { rt.UserId, rt.DeviceId }, "idx_refresh_tokens_user_device_active")
                .HasFilter("(is_revoked = false)");
            b.HasIndex(rt => rt.Token, "ux_refresh_tokens_token_hash").IsUnique();

            b.Property(rt => rt.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
            b.Property(rt => rt.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            b.Property(rt => rt.DeviceId).HasColumnName("device_id");
            b.Property(rt => rt.ExpiresAt).HasColumnName("expires_at");
            b.Property(rt => rt.IsRevoked).HasColumnName("is_revoked");
            b.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");
            b.Property(rt => rt.Token).HasMaxLength(44).IsFixedLength().HasColumnName("token");
            b.Property(rt => rt.UserId).HasColumnName("user_id");

            b.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .HasConstraintName("fk_refresh_tokens_user");
        });
    }
}