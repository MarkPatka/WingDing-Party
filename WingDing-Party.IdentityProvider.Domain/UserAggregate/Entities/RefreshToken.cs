using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.Entities;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    public UserId UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsActive => RevokedAt == null && DateTime.UtcNow <= ExpiresAt;

    private RefreshToken() { } // EF Core

    private RefreshToken(RefreshTokenId id, UserId userId, string token, DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public static RefreshToken Create(UserId userId, string token, DateTime expiresAt)
    {
        return new RefreshToken(
            RefreshTokenId.CreateUnique(),
            userId,
            token,
            expiresAt);
    }

    public void Revoke()
    {
        if (RevokedAt != null)
            return;

        RevokedAt = DateTime.UtcNow;
    }
}