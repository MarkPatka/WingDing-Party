using MediatR;
using UserService.Application.UserProfileManagement.Common;

namespace UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;

public record CreateUserProfileCommand(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate) 
    : IRequest<CreateUserProfileResult>;