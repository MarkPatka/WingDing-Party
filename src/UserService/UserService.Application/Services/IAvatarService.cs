using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.Services;

public interface IAvatarService
{
    Task<Avatar> InsertAsync(Avatar avatar, CancellationToken cancellationToken = default);
    Task UpdateAsync(Avatar avatar, CancellationToken cancellationToken = default);
    Task DeleteAsync(Avatar avatar, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Avatar>> GetAvatarsByUserAsync(UserId userId,
        CancellationToken cancellationToken = default);
}