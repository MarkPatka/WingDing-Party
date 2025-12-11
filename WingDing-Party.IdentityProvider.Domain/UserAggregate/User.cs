using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.DomainEvents;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.Entities;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate;


public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}


//public sealed class User : AggregateRoot<UserId>
//{
//    public string FirstName { get; private set; }
//    public string LastName { get; private set; }

//    // IDENTITY
//    public Email Email { get; private set; }
//    public string NormalizedEmail { get; private set; }
//    public bool EmailConfirmed { get; private set; }

//    // SECURITY
//    public PasswordHash PasswordHash { get; private set; }
//    public SecurityStamp SecurityStamp { get; private set; }
//    public DateTime? LockoutEnd { get; private set; }
//    public int AccessFailedCount { get; private set; }
//    public bool LockoutEnabled { get; private set; }

//    // 2FA
//    public bool TwoFactorEnabled { get; private set; }
//    public string? TwoFactorSecretKey { get; private set; }

//    public PhoneNumber? PhoneNumber { get; private set; }
//    public bool PhoneNumberConfirmed { get; private set; }

//    // ROLES
//    private readonly List<UserRole> _userRoles = [];
//    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

//    // Refresh токены
//    private readonly List<RefreshToken> _refreshTokens = [];
//    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

//    public DateTime CreatedAt { get; private set; }
//    public DateTime? UpdatedAt { get; private set; }

//    private User() { }

//    public static User Create(Email email, PasswordHash passwordHash)
//    {
//        var user = new User
//        {
//            Id = UserId.CreateUnique(),
//            Email = email,
//            NormalizedEmail = email.Value.ToUpperInvariant(),
//            EmailConfirmed = false,
//            PasswordHash = passwordHash,
//            SecurityStamp = SecurityStamp.Generate(),
//            LockoutEnabled = true,
//            AccessFailedCount = 0,
//            TwoFactorEnabled = false,
//            CreatedAt = DateTime.UtcNow
//        };

//        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email));

//        return user;
//    }
//}


