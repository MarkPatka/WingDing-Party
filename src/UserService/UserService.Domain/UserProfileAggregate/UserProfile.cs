using System.ComponentModel.DataAnnotations.Schema;
using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.DomainEvents;
using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate;

public sealed partial class UserProfile : AggregateRoot<UserId>
{
    private List<string> _interests = new();
    
    private List<Avatar> _avatars = new();
    
    public string DisplayName { get; private set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;

    [NotMapped] public AvatarCollection Avatars => new AvatarCollection(_avatars);

    public IReadOnlyList<string> Interests => _interests;
    public DateTime? BirthDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private UserProfile()
    {
    }

    private UserProfile(
        UserId id,
        string displayName,
        string bio,
        IReadOnlyList<string> interests,
        DateTime? birthDate,
        DateTime createdAt,
        DateTime? updatedAt
    )
        : base(id)
    {
        DisplayName = displayName;
        Bio = bio;
        _interests = interests.ToList();
        BirthDate = birthDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserProfile Create(
        string displayName,
        string bio,
        IReadOnlyList<string> interests,
        DateTime? birthDate)
    {
        UserProfile userProfile = new(
            UserId.CreateUnique(),
            displayName,
            bio,
            interests,
            birthDate,
            DateTime.UtcNow,
            DateTime.UtcNow);
        userProfile.AddDomainEvent(new UserProfileCreatedEvent(userProfile.Id, userProfile.CreatedAt));
        return userProfile;
    }

    public void Update(
        string displayName,
        string bio,
        IReadOnlyList<string> interests,
        DateTime? birthDate)
    {
        SetBirthDate(birthDate);
        SetInterests(interests);
        SetBio(bio);
        SetDisplayName(displayName);
    }

    private void SetBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetInterests(IReadOnlyList<string> interests)
    {
        _interests = interests.ToList();
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetBio(string bio)
    {
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDisplayName(string displayName)
    {
        DisplayName = displayName;
        UpdatedAt = DateTime.UtcNow;
    }
}