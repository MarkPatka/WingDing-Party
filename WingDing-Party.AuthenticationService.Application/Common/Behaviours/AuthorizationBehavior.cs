using MediatR;

namespace WingDing_Party.AuthenticationService.Application.Common.Behaviours;

public class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    //where TResponse : ...
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        //if (request is IAuthorizableRequest authorizableRequest)
        //{
        //    var userHasPermission = await _authorizationService
        //        .CheckPermissionAsync(userId, authorizableRequest.RequiredPermission);

        //    if (!userHasPermission)
        //        throw new UnauthorizedAccessException();
        //}

        return await next(cancellationToken);
    }
}