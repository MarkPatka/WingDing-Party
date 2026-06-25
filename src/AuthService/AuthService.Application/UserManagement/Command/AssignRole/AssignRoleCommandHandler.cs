using AuthService.Application.Common.Errors;
using AuthService.Application.Persistence;
using AuthService.Application.Services;
using AuthService.Application.UserManagement.Common;
using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Domain.Enumerations;
using AuthService.Domain.ValueObjects.Ids;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.UserManagement.Command.AssignRole;

public sealed class AssignRoleCommandHandler(IAuthDbContext authDbContext, IPermissionCache permissionCache)
    : IRequestHandler<AssignRoleCommand, AssignRoleResult>
{
    private readonly IAuthDbContext _authDb = authDbContext;
    private readonly IPermissionCache _permissionCache = permissionCache;

    public async Task<AssignRoleResult> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        User user = await _authDb.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == UserId.Create(request.UserId), cancellationToken)
            ?? throw ServiceErrors.UserNotFound(request.UserId);

        // Имя роли уже проверено валидатором; перехватываем на случай обхода пайплайна.
        RoleType roleType;
        try
        {
            roleType = Enumeration.GetFromName<RoleType>(request.Role);
        }
        catch (ApplicationException)
        {
            throw ServiceErrors.InvalidRole(request.Role);
        }

        Role role = Role.Create(roleType);

        // Идемпотентность: роль уже есть — ничего не меняем.
        if (user.HasRole(role))
            return new AssignRoleResult([.. user.Roles.Select(r => r.RoleType.Name)]);

        // Роль засижена (Id == RoleType.Id). Цепляем как Unchanged, чтобы EF
        // вставил только строку в user_roles, а не новую запись в roles (иначе конфликт PK).
        _authDb.Roles.Attach(role);
        user.AddRole(role);
        await _authDb.SaveChangesAsync(cancellationToken);

        // Сбрасываем кэш ролей/прав, чтобы новые права подхватились немедленно, а не через TTL.
        await _permissionCache.InvalidateAsync(user.IdentityId);

        return new AssignRoleResult([.. user.Roles.Select(r => r.RoleType.Name)]);
    }
}
