using ClubService.Application.Persistence;
using ClubService.Application.Persistence.Specifications.Clubs;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.Entities;
using ClubService.Domain.ClubAggregate.ValueObjects;

namespace ClubService.Infrastructure.Services;

public class ClubService : IClubService
{
    private readonly IRepository<Club, ClubId> _repository;

    public ClubService(IRepository<Club, ClubId> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Club>> SearchClubsAsync(string name, IEnumerable<string>? interests = null,
        OwnerId? ownerId = null)
    {
        var spec = new SearchClubsSpec(name, interests ?? [], ownerId);
        return await _repository.ListAsync(spec);
    }
    
    public async Task<bool> GetAnyClubsAsync(string name, IEnumerable<string>? interests = null,
        OwnerId? ownerId = null)
    {
        var spec = new GetAnyClubsSpec(name, interests ?? [], ownerId);
        return await _repository.AnyAsync(spec);
    }

    public async Task<Club?> GetClubByIdAsync(ClubId clubId)
    {
        var spec = new ClubByIdSpec(clubId);
        return await _repository.FirstOrDefaultAsync(spec);
    }

    public async Task<IEnumerable<Club>> GetClubsByUserAsync(UserId userId)
    {
        var spec = new ClubsByUserSpec(userId);
        return await _repository.ListAsync(spec);
    }

    public async Task<IEnumerable<ClubMember>> GetClubMembersAsync(ClubId clubId)
    {
        Club? club = await GetClubByIdAsync(clubId);
        return club is not null ? club.ClubMembers : Enumerable.Empty<ClubMember>();
    }

    public async Task<bool> IsMemberParticipatedAsync(ClubId clubId, UserId userId)
    {
        Club? club = await GetClubByIdAsync(clubId);
        return club is not null && club.ClubMembers.Any(x => x.UserId == userId);
    }

    public async Task<Club> AddAsync(Club club)
    {
        return await _repository.AddAsync(club);
    }

    public async Task AddMember(Club club, UserId userId, DateTime joinTime)
    {
        club.AddMember(userId, joinTime);
        await _repository.UpdateAsync(club);
    }

    public async Task DeleteMember(Club club, UserId userId, DateTime joinTime)
    {
        club.DeleteMember(userId, joinTime);
        await _repository.UpdateAsync(club);
    }

    public async Task DeleteAsync(Club club)
    {
        await _repository.DeleteAsync(club);
    }

    public async Task UpdateAsync(Club club)
    {
        await _repository.UpdateAsync(club);
    }
}