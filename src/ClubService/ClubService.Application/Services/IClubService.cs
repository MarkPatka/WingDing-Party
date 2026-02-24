using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;

namespace ClubService.Application.Services;

public interface IClubService
{
    Task<IEnumerable<Club>> SearchClubsAsync(string name, IEnumerable<string>? interests = null,
        OwnerId? ownerId = null);

    Task<bool> GetAnyClubsAsync(string name, IEnumerable<string>? interests = null,
        OwnerId? ownerId = null);

    Task<Club?> GetClubByIdAsync(ClubId clubId);
    Task<IEnumerable<Club>> GetClubsByUserAsync(UserId userId);
    Task<IEnumerable<ClubMember>> GetClubMembersAsync(ClubId clubId);
    Task<bool> IsMemberParticipatedAsync(ClubId clubId, UserId userId);

    Task<Club> AddAsync(Club club);

    Task AddMember(Club club, UserId userId, DateTime joinTime);
    Task DeleteMember(Club clubId, UserId userId, DateTime joinTime);
    Task DeleteAsync(Club club);
    Task UpdateAsync(Club club);
}