using MediatR;
using UserService.Application.UserProfileManagement.Common;

namespace UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;

public record GetUserProfileQuery(Guid UserId) : IRequest<GetUserProfileResult>;