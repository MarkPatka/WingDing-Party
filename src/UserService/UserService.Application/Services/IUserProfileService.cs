using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.Services;

public interface IUserProfileService
{
    Task<UserProfile> InsertAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<string?>> GetUsersInterestsAsync(UserId userId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetUserByNameAsync(string displayName, CancellationToken cancellationToken = default);

    Task<UserProfile?> GetUserProfileByIdAsync(UserId userId, CancellationToken cancellationToken = default);
}