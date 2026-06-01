# Graph Report - src/AuthService  (2026-05-17)

## Corpus Check
- 39 files · ~99,999 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 220 nodes · 197 edges · 49 communities (16 shown, 33 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 13 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 16|Community 16]]
- [[_COMMUNITY_Community 17|Community 17]]
- [[_COMMUNITY_Community 18|Community 18]]
- [[_COMMUNITY_Community 19|Community 19]]
- [[_COMMUNITY_Community 20|Community 20]]
- [[_COMMUNITY_Community 21|Community 21]]
- [[_COMMUNITY_Community 22|Community 22]]
- [[_COMMUNITY_Community 23|Community 23]]
- [[_COMMUNITY_Community 24|Community 24]]
- [[_COMMUNITY_Community 25|Community 25]]
- [[_COMMUNITY_Community 26|Community 26]]
- [[_COMMUNITY_Community 27|Community 27]]
- [[_COMMUNITY_Community 28|Community 28]]
- [[_COMMUNITY_Community 29|Community 29]]
- [[_COMMUNITY_Community 30|Community 30]]
- [[_COMMUNITY_Community 31|Community 31]]
- [[_COMMUNITY_Community 32|Community 32]]
- [[_COMMUNITY_Community 33|Community 33]]
- [[_COMMUNITY_Community 34|Community 34]]
- [[_COMMUNITY_Community 35|Community 35]]
- [[_COMMUNITY_Community 36|Community 36]]
- [[_COMMUNITY_Community 37|Community 37]]
- [[_COMMUNITY_Community 38|Community 38]]
- [[_COMMUNITY_Community 39|Community 39]]
- [[_COMMUNITY_Community 40|Community 40]]
- [[_COMMUNITY_Community 41|Community 41]]
- [[_COMMUNITY_Community 42|Community 42]]
- [[_COMMUNITY_Community 43|Community 43]]
- [[_COMMUNITY_Community 44|Community 44]]
- [[_COMMUNITY_Community 45|Community 45]]
- [[_COMMUNITY_Community 46|Community 46]]
- [[_COMMUNITY_Community 47|Community 47]]

## God Nodes (most connected - your core abstractions)
1. `Enumeration` - 11 edges
2. `DependencyInjection` - 7 edges
3. `UsersController` - 7 edges
4. `DependencyInjection` - 7 edges
5. `User` - 6 edges
6. `ValueObject` - 5 edges
7. `UserId` - 5 edges
8. `AdminAuthorizationDelegatingHandler` - 5 edges
9. `JwtService` - 5 edges
10. `Permission-Based Authorization Pattern` - 5 edges

## Surprising Connections (you probably didn't know these)
- `Permission-Based Authorization Pattern` --rationale_for--> `PermissionConfiguration`  [INFERRED]
  src/AuthService/CLAUDE.md → src/AuthService/AuthService.Infrastructure/Persistence/ModelsConfiguration/PermissionConfiguration.cs
- `Permissions Constants` --rationale_for--> `Permission-Based Authorization Pattern`  [EXTRACTED]
  src/AuthService/AuthService.Contracts/Constants/Permissions.cs → src/AuthService/CLAUDE.md
- `Permission-Based Authorization Pattern` --rationale_for--> `RoleConfiguration`  [INFERRED]
  src/AuthService/CLAUDE.md → src/AuthService/AuthService.Infrastructure/Persistence/ModelsConfiguration/RoleConfiguration.cs
- `Keycloak Integration Pattern` --rationale_for--> `JwtService`  [INFERRED]
  src/AuthService/CLAUDE.md → src/AuthService/AuthService.Infrastructure/Services/JwtService.cs
- `ClaimsPrincipalExtensions` --conceptually_related_to--> `CustomClaimsTransformation Enriches ClaimsPrincipal from DB`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Common/Extensions/ClaimsPrincipalExtensions.cs → src/AuthService/CLAUDE.md

## Hyperedges (group relationships)
- **DI composition root pipeline** — api_program, api_di_addpresentation, infra_di_addinfrastructure [EXTRACTED 1.00]
- **Permission Oracle gRPC flow** — api_program, api_permission_grpc_service, concept_permission_oracle [EXTRACTED 1.00]
- **Keycloak user registration flow** — api_users_controller, infra_authentication_service, domain_user_create_factory, concept_keycloak_identity_link [INFERRED 0.85]

## Communities (49 total, 33 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.12
Nodes (19): AddLogging (Serilog split sinks), AddMappings (Mapster scan), AddPresentation DI Extension, BindConfigurations (root-bind options), PermissionGrpcService, AuthService.Api Program (composition root), UsersController (register/login/me), Keycloak Identity Link (sub -> User.IdentityId) (+11 more)

### Community 1 - "Community 1"
Cohesion: 0.14
Nodes (4): RoleId, UserId, ValueObject, RolePermission

### Community 2 - "Community 2"
Cohesion: 0.2
Nodes (4): DependencyInjection, HttpClient, IAuthenticationService, AuthenticationService

### Community 3 - "Community 3"
Cohesion: 0.19
Nodes (13): AuthDbContext, Permissions Constants, Entity<TId>, Enumeration, Permission (Enumeration), Role, RoleType (Enumeration), InitialCreate_WithRolesAndPermissions Migration (+5 more)

### Community 5 - "Community 5"
Cohesion: 0.18
Nodes (5): IEntityTypeConfiguration, PermissionConfiguration, RoleConfiguration, UserConfiguration, object

### Community 6 - "Community 6"
Cohesion: 0.24
Nodes (3): Entity, ValueObject, IEquatable

### Community 7 - "Community 7"
Cohesion: 0.22
Nodes (5): AdminAuthorizationDelegatingHandler, DelegatingHandler, IJwtService, KeycloakOptions, JwtService

### Community 8 - "Community 8"
Cohesion: 0.22
Nodes (4): Role, User, Entity, List

### Community 9 - "Community 9"
Cohesion: 0.22
Nodes (9): IAuthenticationService, IJwtService, ClaimsPrincipalExtensions, AuthService CLAUDE.md, JwtService, CustomClaimsTransformation Enriches ClaimsPrincipal from DB, Keycloak Integration Pattern, Redis Role Cache (TTL 5min, key auth:roles-{identityId}) (+1 more)

### Community 11 - "Community 11"
Cohesion: 0.25
Nodes (4): ControllerBase, UsersController, IMapper, ISender

### Community 12 - "Community 12"
Cohesion: 0.33
Nodes (3): Migration, AuthService.Infrastructure.Persistence.Migrations, InitialCreate_WithRolesAndPermissions

### Community 13 - "Community 13"
Cohesion: 0.6
Nodes (3): Enumeration, Permission, RoleType

### Community 14 - "Community 14"
Cohesion: 0.4
Nodes (3): JwtBearerOptionsSetup, AuthenticationOptions, IConfigureNamedOptions

### Community 16 - "Community 16"
Cohesion: 0.5
Nodes (3): UserContext, IHttpContextAccessor, IUserContext

### Community 19 - "Community 19"
Cohesion: 0.5
Nodes (4): AdminAuthorizationDelegatingHandler, AuthorizationToken DTO, CredentialRepresentationModel DTO, KeycloakOptions

## Knowledge Gaps
- **49 isolated node(s):** `IUserContext`, `Permissions`, `AuthenticationOptions`, `IHttpContextAccessor`, `AuthDatabaseOptions` (+44 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **33 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `JwtService` connect `Community 7` to `Community 2`?**
  _High betweenness centrality (0.006) - this node is a cross-community bridge._
- **Why does `Permission-Based Authorization Pattern` connect `Community 3` to `Community 9`?**
  _High betweenness centrality (0.006) - this node is a cross-community bridge._
- **What connects `IUserContext`, `Permissions`, `AuthenticationOptions` to the rest of the system?**
  _49 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.14 - nodes in this community are weakly interconnected._