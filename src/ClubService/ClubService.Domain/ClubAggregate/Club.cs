using ClubService.Domain.ClubAggregate.DomainEvents;
using ClubService.Domain.ClubAggregate.ValueObjects;
using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate;

public sealed class Club : AggregateRoot<ClubId>
{
    private readonly List<ClubMember> _members = [];
    private List<string> _interests = new();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public OwnerId Owner { get; private set; }
    public IReadOnlyCollection<string> Interests => _interests;
    
    public IReadOnlyCollection<ClubMember> ClubMembers => _members.AsReadOnly();

    public bool IsPublic { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Club() { }

    private Club(
        ClubId id,
        string name,
        string description,
        IEnumerable<string> interests,
        OwnerId owner,
        bool isPublic,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        Id = id;
        Name = name;
        Description = description;
        _interests = interests.ToList();
        Owner = owner;
        IsPublic = isPublic;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Club Create(
        string name,
        string description,
        IEnumerable<string> interests,
        OwnerId owner,
        bool isPublic)

    {
        var club = new Club(
            ClubId.CreateUnique(),
            name, description, interests, owner, isPublic, DateTime.UtcNow, DateTime.UtcNow
        );
        club.AddDomainEvent(new ClubCreatedDomainEvent(club.Id, club.CreatedAt));
        return club;
    }

    public void Update(
        string name,
        string description,
        IEnumerable<string> interests,
        OwnerId owner,
        bool isPublic)

    {
        SetName(name);
        SetDescription(description);
        SetInterests(interests);
        SetOwner(owner);
        SetIsPublic(isPublic);
    }

    private void SetName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetIsPublic(bool isPublic)
    {
        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetInterests(IEnumerable<string> interests)
    {
        _interests = interests.ToList();
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetOwner(OwnerId owner)
    {
        Owner = owner;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMember(UserId userId, DateTime joinedAt)
    {
        if (_members.FirstOrDefault(m => m.UserId == userId) == null)
            _members.Add(ClubMember.Create(Id, userId, joinedAt));
    }

    public void DeleteMember(UserId userId, DateTime joinedAt)
    {
        var existMember = _members.FirstOrDefault(m => m.UserId == userId);
        if (existMember != null)
        {
            _members.Remove(existMember);
        }
    }
}