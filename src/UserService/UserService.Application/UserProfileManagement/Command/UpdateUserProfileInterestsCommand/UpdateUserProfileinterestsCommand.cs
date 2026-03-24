using MediatR;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;

public record UpdateUserProfileInterestsCommand(
    Guid UserId,
    IReadOnlyList<string> Interests)
    : IRequest<UpdateUserProfileInterestsResult>;