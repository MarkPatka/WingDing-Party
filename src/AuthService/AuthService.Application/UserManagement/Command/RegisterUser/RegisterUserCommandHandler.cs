using AuthService.Application.Persistence;
using AuthService.Application.Services;
using AuthService.Application.UserManagement.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuthService.Application.UserManagement.Command.RegisterUser;

public sealed class RegisterUserCommandHandler(IAuthDbContext authDbContext, IAuthenticationService authService) 
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IAuthDbContext _authDb = authDbContext;
    private readonly IAuthenticationService _authService = authService;

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = User.Create(request.FirstName, request.LastName, request.Email);
        _authDb.Users.Add(newUser);

        // EF попытается вставить роль Registered повторно.
        // User.Create кладёт в _roles новый объект Role(Registered) с Id = 2, который уже в таблице roles.
        // При Users.Add(user) + SaveChanges EF пойдёт INSERT в roles(id = 2)
        // -> конфликт PK.
        foreach (Role role in newUser.Roles)
        {
            _authDb.Roles.Entry(role).State = EntityState.Unchanged;
        }

        string identityId = await _authService.RegisterAsync(newUser, request.Password, cancellationToken);

        newUser.SetIdentityId(identityId);

        await _authDb.SaveChangesAsync(cancellationToken);
        return new RegisterUserResult(newUser.Id.Value, newUser.Email); 
    }
}

