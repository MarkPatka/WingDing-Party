# Graph Report - src/AuthService  (2026-05-08)

## Corpus Check
- 65 files · ~6,727 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 253 nodes · 241 edges · 51 communities (31 shown, 20 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 19 edges (avg confidence: 0.88)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_API & Application Interfaces|API & Application Interfaces]]
- [[_COMMUNITY_Infrastructure Core Services|Infrastructure Core Services]]
- [[_COMMUNITY_JWT Authentication Setup|JWT Authentication Setup]]
- [[_COMMUNITY_Admin Auth Delegating Handler|Admin Auth Delegating Handler]]
- [[_COMMUNITY_Domain Value Objects (IDs)|Domain Value Objects (IDs)]]
- [[_COMMUNITY_Enumeration Base Class|Enumeration Base Class]]
- [[_COMMUNITY_EF Core Model Configuration|EF Core Model Configuration]]
- [[_COMMUNITY_Domain Abstractions|Domain Abstractions]]
- [[_COMMUNITY_Domain Entities (Role, User)|Domain Entities (Role, User)]]
- [[_COMMUNITY_API Dependency Injection|API Dependency Injection]]
- [[_COMMUNITY_Users Controller|Users Controller]]
- [[_COMMUNITY_Authorization DB Service|Authorization DB Service]]
- [[_COMMUNITY_EF Core Migration|EF Core Migration]]
- [[_COMMUNITY_Domain Enumerations|Domain Enumerations]]
- [[_COMMUNITY_Permission Auth Handler|Permission Auth Handler]]
- [[_COMMUNITY_Permission Policy Provider|Permission Policy Provider]]
- [[_COMMUNITY_Permissions & Config Constants|Permissions & Config Constants]]
- [[_COMMUNITY_DB Schema Snapshot|DB Schema Snapshot]]
- [[_COMMUNITY_User Context|User Context]]
- [[_COMMUNITY_Custom Claims Transformation|Custom Claims Transformation]]
- [[_COMMUNITY_Claims Principal Extensions|Claims Principal Extensions]]
- [[_COMMUNITY_Auth DB Context|Auth DB Context]]
- [[_COMMUNITY_Migration Designer|Migration Designer]]
- [[_COMMUNITY_Application DI|Application DI]]
- [[_COMMUNITY_IAuthentication Service|IAuthentication Service]]
- [[_COMMUNITY_IJwt Service|IJwt Service]]
- [[_COMMUNITY_Has Permission Attribute|Has Permission Attribute]]
- [[_COMMUNITY_Permission Requirement|Permission Requirement]]
- [[_COMMUNITY_Program & DI Bootstrap|Program & DI Bootstrap]]
- [[_COMMUNITY_IUser Context Interface|IUser Context Interface]]
- [[_COMMUNITY_Authentication Options|Authentication Options]]
- [[_COMMUNITY_Keycloak Options|Keycloak Options]]
- [[_COMMUNITY_Redis Options|Redis Options]]
- [[_COMMUNITY_Authentication Token DTO|Authentication Token DTO]]
- [[_COMMUNITY_Credential Representation DTO|Credential Representation DTO]]
- [[_COMMUNITY_User Representation DTO|User Representation DTO]]
- [[_COMMUNITY_Role Permission Value Object|Role Permission Value Object]]
- [[_COMMUNITY_ValueObject Base Class|ValueObject Base Class]]

## God Nodes (most connected - your core abstractions)
1. `Enumeration` - 11 edges
2. `Infrastructure DependencyInjection` - 11 edges
3. `DependencyInjection` - 8 edges
4. `DependencyInjection` - 7 edges
5. `UsersController` - 7 edges
6. `User` - 6 edges
7. `AuthorizationService` - 6 edges
8. `AuthorizationService` - 6 edges
9. `ValueObject` - 5 edges
10. `UserId` - 5 edges

## Surprising Connections (you probably didn't know these)
- `PermissionConfiguration` --rationale_for--> `Permission-Based Authorization Pattern`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Persistence/ModelsConfiguration/PermissionConfiguration.cs → src/AuthService/CLAUDE.md
- `AuthenticationService` --rationale_for--> `Keycloak Integration Pattern`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Services/AuthenticationService.cs → src/AuthService/CLAUDE.md
- `JwtService` --rationale_for--> `Keycloak Integration Pattern`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Services/JwtService.cs → src/AuthService/CLAUDE.md
- `Keycloak Integration Pattern` --rationale_for--> `IJwtService`  [EXTRACTED]
  src/AuthService/CLAUDE.md → src/AuthService/AuthService.Application/Services/IJwtService.cs
- `RoleConfiguration` --rationale_for--> `Permission-Based Authorization Pattern`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Persistence/ModelsConfiguration/RoleConfiguration.cs → src/AuthService/CLAUDE.md

## Hyperedges (group relationships)
- **Clean Architecture Layers** — api_dependencyinjection, application_dependencyinjection, domain_entity, domain_role, domain_user [INFERRED 0.95]
- **Domain Abstraction Base Classes** — domain_entity, domain_enumeration, domain_valueobject [EXTRACTED 1.00]
- **Permission System** — domain_permission, contracts_permissions, api_userscontroller, rationale_permission_based_authz [EXTRACTED 1.00]
- **Permission-Based Authorization Pipeline** — haspermissionattribute_haspermissionattribute, permissionauthorizationpolicyprovider_permissionauthorizationpolicyprovider, permissionrequirement_permissionrequirement, permissionauthorizationhandler_permissionauthorizationhandler, authorizationservice_authorizationservice [EXTRACTED 1.00]
- **Keycloak Integration Components** — adminauthorizationdelegatinghandler_adminauthorizationdelegatinghandler, keycloakoptions_keycloakoptions, authorizationtoken_authorizationtoken, credentialrepresentationmodel_credentialrepresentationmodel [EXTRACTED 1.00]
- **Infrastructure Configuration Options** — authdatabaseoptions_authdatabaseoptions, authenticationoptions_authenticationoptions, keycloakoptions_keycloakoptions, redisoptions_redisoptions [INFERRED 0.95]
- **Claims Enrichment Flow** — customclaimstransformation_customclaimstransformation, authorizationservice_authorizationservice, userrolesresponse_userrolesresponse [EXTRACTED 1.00]
- **EF Core Persistence Configuration (AuthDbContext + Configurations)** — authdbcontext_authdbcontext, permissionconfiguration_permissionconfiguration, roleconfiguration_roleconfiguration, userconfiguration_userconfiguration [EXTRACTED 1.00]
- **Keycloak HTTP Services (AuthenticationService + JwtService)** — authenticationservice_authenticationservice, jwtservice_jwtservice, userrepresentationmodel_userrepresentationmodel [EXTRACTED 1.00]
- **Permission-Based Authorization System** — rationale_permission_based_authz, permissionconfiguration_permissionconfiguration, roleconfiguration_roleconfiguration, claimsprincipalextensions_claimsprincipalextensions, rationale_custom_claims_transformation, rationale_redis_role_cache [INFERRED 0.85]

## Communities (51 total, 20 thin omitted)

### Community 0 - "API & Application Interfaces"
Cohesion: 0.1
Nodes (27): UsersController, IAuthenticationService, IJwtService, IUserContext, AuthDbContext, AuthenticationService, ClaimsPrincipalExtensions, AuthService CLAUDE.md (+19 more)

### Community 1 - "Infrastructure Core Services"
Cohesion: 0.17
Nodes (18): AdminAuthorizationDelegatingHandler, AuthDatabaseOptions, AuthenticationOptions, AuthorizationService, AuthorizationToken DTO, CredentialRepresentationModel DTO, CustomClaimsTransformation, Infrastructure DependencyInjection (+10 more)

### Community 2 - "JWT Authentication Setup"
Cohesion: 0.18
Nodes (5): JwtBearerOptionsSetup, AuthenticationOptions, DependencyInjection, IAuthenticationService, IConfigureNamedOptions

### Community 3 - "Admin Auth Delegating Handler"
Cohesion: 0.15
Nodes (7): AdminAuthorizationDelegatingHandler, DelegatingHandler, HttpClient, IJwtService, KeycloakOptions, AuthenticationService, JwtService

### Community 4 - "Domain Value Objects (IDs)"
Cohesion: 0.14
Nodes (4): RoleId, UserId, ValueObject, RolePermission

### Community 6 - "EF Core Model Configuration"
Cohesion: 0.18
Nodes (5): IEntityTypeConfiguration, PermissionConfiguration, RoleConfiguration, UserConfiguration, object

### Community 7 - "Domain Abstractions"
Cohesion: 0.24
Nodes (3): Entity, ValueObject, IEquatable

### Community 8 - "Domain Entities (Role, User)"
Cohesion: 0.24
Nodes (4): Role, User, Entity, List

### Community 10 - "Users Controller"
Cohesion: 0.25
Nodes (4): ControllerBase, UsersController, IMapper, ISender

### Community 11 - "Authorization DB Service"
Cohesion: 0.29
Nodes (4): AuthDbContext, AuthorizationService, IDistributedCache, TimeSpan

### Community 12 - "EF Core Migration"
Cohesion: 0.33
Nodes (3): Migration, AuthService.Infrastructure.Persistence.Migrations, InitialCreate_WithRolesAndPermissions

### Community 13 - "Domain Enumerations"
Cohesion: 0.6
Nodes (3): Enumeration, Permission, RoleType

### Community 14 - "Permission Auth Handler"
Cohesion: 0.4
Nodes (3): PermissionAuthorizationHandler, AuthorizationHandler, IServiceProvider

### Community 15 - "Permission Policy Provider"
Cohesion: 0.4
Nodes (3): PermissionAuthorizationPolicyProvider, AuthorizationOptions, DefaultAuthorizationPolicyProvider

### Community 16 - "Permissions & Config Constants"
Cohesion: 0.4
Nodes (3): AuthDatabaseOptions, Permissions, string

### Community 17 - "DB Schema Snapshot"
Cohesion: 0.4
Nodes (3): AuthDbContextModelSnapshot, AuthService.Infrastructure.Persistence.Migrations, ModelSnapshot

### Community 18 - "User Context"
Cohesion: 0.5
Nodes (3): UserContext, IHttpContextAccessor, IUserContext

### Community 28 - "Program & DI Bootstrap"
Cohesion: 0.67
Nodes (3): Api DependencyInjection, Program (Entry Point), Application DependencyInjection

## Knowledge Gaps
- **35 isolated node(s):** `ISender`, `IMapper`, `IUserContext`, `IHttpContextAccessor`, `AuthDbContext` (+30 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **20 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `AuthenticationService` connect `Admin Auth Delegating Handler` to `JWT Authentication Setup`?**
  _High betweenness centrality (0.008) - this node is a cross-community bridge._
- **What connects `ISender`, `IMapper`, `IUserContext` to the rest of the system?**
  _35 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `API & Application Interfaces` be split into smaller, more focused modules?**
  _Cohesion score 0.1 - nodes in this community are weakly interconnected._
- **Should `Domain Value Objects (IDs)` be split into smaller, more focused modules?**
  _Cohesion score 0.14 - nodes in this community are weakly interconnected._