# Graph Report - src/AuthService  (2026-05-06)

## Corpus Check
- Corpus is ~6,252 words - fits in a single context window. You may not need a graph.

## Summary
- 243 nodes · 239 edges · 51 communities (30 shown, 21 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 11 edges (avg confidence: 0.88)
- Token cost: 133,550 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Keycloak Auth Service|Keycloak Auth Service]]
- [[_COMMUNITY_Admin Token Delegation|Admin Token Delegation]]
- [[_COMMUNITY_Claims & Authorization|Claims & Authorization]]
- [[_COMMUNITY_Domain Value Objects|Domain Value Objects]]
- [[_COMMUNITY_JWT Bearer Setup|JWT Bearer Setup]]
- [[_COMMUNITY_Domain Enumeration Base|Domain Enumeration Base]]
- [[_COMMUNITY_EF Core Model Config|EF Core Model Config]]
- [[_COMMUNITY_DDD Abstractions|DDD Abstractions]]
- [[_COMMUNITY_Domain Entities|Domain Entities]]
- [[_COMMUNITY_Database Schema|Database Schema]]
- [[_COMMUNITY_API Startup|API Startup]]
- [[_COMMUNITY_Permission Policy|Permission Policy]]
- [[_COMMUNITY_Infrastructure DI|Infrastructure DI]]
- [[_COMMUNITY_App Service Interfaces|App Service Interfaces]]
- [[_COMMUNITY_User Context|User Context]]
- [[_COMMUNITY_Keycloak Config|Keycloak Config]]
- [[_COMMUNITY_Migrations|Migrations]]
- [[_COMMUNITY_DB Table Schema|DB Table Schema]]
- [[_COMMUNITY_Authorization Handler|Authorization Handler]]
- [[_COMMUNITY_Role Type Model|Role Type Model]]
- [[_COMMUNITY_Redis Cache Config|Redis Cache Config]]
- [[_COMMUNITY_Credential DTOs|Credential DTOs]]
- [[_COMMUNITY_Claim Transforms|Claim Transforms]]
- [[_COMMUNITY_Permission Requirement|Permission Requirement]]
- [[_COMMUNITY_User Roles Response|User Roles Response]]
- [[_COMMUNITY_Contract Models|Contract Models]]
- [[_COMMUNITY_Misc Component 26|Misc Component 26]]
- [[_COMMUNITY_Misc Component 27|Misc Component 27]]
- [[_COMMUNITY_Misc Component 28|Misc Component 28]]
- [[_COMMUNITY_Misc Component 29|Misc Component 29]]
- [[_COMMUNITY_Misc Component 30|Misc Component 30]]
- [[_COMMUNITY_Misc Component 31|Misc Component 31]]
- [[_COMMUNITY_Misc Component 32|Misc Component 32]]
- [[_COMMUNITY_Misc Component 33|Misc Component 33]]
- [[_COMMUNITY_Misc Component 34|Misc Component 34]]
- [[_COMMUNITY_Misc Component 35|Misc Component 35]]
- [[_COMMUNITY_Misc Component 36|Misc Component 36]]
- [[_COMMUNITY_Misc Component 50|Misc Component 50]]

## God Nodes (most connected - your core abstractions)
1. `Enumeration` - 11 edges
2. `Infrastructure DependencyInjection` - 11 edges
3. `DependencyInjection` - 7 edges
4. `DependencyInjection` - 7 edges
5. `User` - 6 edges
6. `AuthorizationService` - 6 edges
7. `User Entity` - 6 edges
8. `AuthenticationService` - 6 edges
9. `ValueObject` - 5 edges
10. `UserId` - 5 edges

## Surprising Connections (you probably didn't know these)
- `User Entity` --conceptually_related_to--> `IUserContext Interface`  [INFERRED]
  src/AuthService/AuthService.Domain/Entities/User.cs → src/AuthService/AuthService.Application/Common/Interfaces/IUserContext.cs
- `Custom Claims Transformation` --conceptually_related_to--> `UserId Value Object`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Authorization/CustomClaimsTransformation.cs → src/AuthService/AuthService.Domain/ValueObjects/Ids/UserId.cs
- `Authorization Service` --conceptually_related_to--> `Redis Options`  [INFERRED]
  src/AuthService/AuthService.Infrastructure/Authorization/AuthorizationService.cs → src/AuthService/AuthService.Infrastructure/Common/Configuration/RedisOptions.cs
- `AdminAuthorizationDelegatingHandler` --references--> `KeycloakOptions`  [EXTRACTED]
  AuthService.Infrastructure/Authentication/AdminAuthorizationDelegatingHandler.cs → AuthService.Infrastructure/Services/JwtService.cs
- `AuthenticationService` --references--> `HttpClient`  [EXTRACTED]
  AuthService.Infrastructure/Services/AuthenticationService.cs → AuthService.Infrastructure/Services/JwtService.cs

## Hyperedges (group relationships)
- **DDD Domain Abstractions Pattern** — entity_abstract, enumeration_abstract, valueobject_abstract [INFERRED 0.95]
- **Application Service Interface Layer** — iauthenticationservice_interface, ijwtservice_interface, iusercontext_interface [INFERRED 0.95]
- **User-Role Domain Aggregate** — user_entity, role_entity, permission_enumeration, roletype_enumeration [EXTRACTED 1.00]
- **Application Startup Service Registration Pipeline** — program_entrypoint, apidi_dependencyinjection, appdi_dependencyinjection [EXTRACTED 1.00]
- **Permission Authorization Pipeline** — haspermissionattribute_HasPermissionAttribute, permissionauthorizationpolicyprovider_PermissionAuthorizationPolicyProvider, permissionrequirement_PermissionRequirement, permissionauthorizationhandler_PermissionAuthorizationHandler, authorizationservice_AuthorizationService [INFERRED 0.95]
- **JWT Claims Enrichment Flow** — customclaimstransformation_CustomClaimsTransformation, claimsprincipalextensions_ClaimsPrincipalExtensions, authorizationservice_AuthorizationService, userrolesresponse_UserRolesResponse [EXTRACTED 0.95]
- **Infrastructure Configuration Options** — authdatabaseoptions_AuthDatabaseOptions, authenticationoptions_AuthenticationOptions, keycloakoptions_KeycloakOptions, redisoptions_RedisOptions [INFERRED 0.85]
- **EF Core Persistence Layer Configuration** — authdbcontext_AuthDbContext, permissionconfiguration_PermissionConfiguration, roleconfiguration_RoleConfiguration, userconfiguration_UserConfiguration [INFERRED 0.95]
- **Auth Database Schema Tables** — db_table_users, db_table_roles, db_table_permissions, db_table_role_permissions, db_table_user_roles [EXTRACTED 1.00]
- **Keycloak Integration Services** — authenticationservice_AuthenticationService, jwtservice_JwtService, keycloak_admin_api, keycloak_token_api [INFERRED 0.95]

## Communities (51 total, 21 thin omitted)

### Community 0 - "Keycloak Auth Service"
Cohesion: 0.16
Nodes (18): Admin Authorization Delegating Handler, AuthenticationService, Authorization Token DTO, Credential Representation Model DTO, Entity<Tid> Abstract Base, Enumeration Abstract Base, IAuthenticationService Interface, IJwtService Interface (+10 more)

### Community 1 - "Admin Token Delegation"
Cohesion: 0.15
Nodes (7): AdminAuthorizationDelegatingHandler, DelegatingHandler, HttpClient, IJwtService, KeycloakOptions, AuthenticationService, JwtService

### Community 2 - "Claims & Authorization"
Cohesion: 0.23
Nodes (15): Auth Database Options, Authentication Options, Authorization Service, Claims Principal Extensions, Custom Claims Transformation, Infrastructure DependencyInjection, Has Permission Attribute, JWT Bearer Options Setup (+7 more)

### Community 3 - "Domain Value Objects"
Cohesion: 0.14
Nodes (4): RoleId, UserId, ValueObject, RolePermission

### Community 4 - "JWT Bearer Setup"
Cohesion: 0.19
Nodes (5): JwtBearerOptionsSetup, AuthenticationOptions, DependencyInjection, IAuthenticationService, IConfigureNamedOptions

### Community 6 - "EF Core Model Config"
Cohesion: 0.18
Nodes (5): IEntityTypeConfiguration, PermissionConfiguration, RoleConfiguration, UserConfiguration, object

### Community 7 - "DDD Abstractions"
Cohesion: 0.24
Nodes (3): Entity, ValueObject, IEquatable

### Community 8 - "Domain Entities"
Cohesion: 0.24
Nodes (4): Role, User, Entity, List

### Community 9 - "Database Schema"
Cohesion: 0.38
Nodes (10): AuthDbContext, DB Table: permissions, DB Table: role_permissions, DB Table: roles, DB Table: user_roles, DB Table: users, InitialCreate_WithRolesAndPermissions Migration, PermissionConfiguration (+2 more)

### Community 11 - "Permission Policy"
Cohesion: 0.29
Nodes (4): AuthDbContext, AuthorizationService, IDistributedCache, TimeSpan

### Community 12 - "Infrastructure DI"
Cohesion: 0.33
Nodes (3): Migration, AuthService.Infrastructure.Persistence.Migrations, InitialCreate_WithRolesAndPermissions

### Community 13 - "App Service Interfaces"
Cohesion: 0.33
Nodes (6): AuthService.Api DependencyInjection, AuthService.Application DependencyInjection, AuthDatabaseOptions Configuration, AuthenticationOptions Configuration, KeycloakOptions Configuration, AuthService API Entry Point

### Community 14 - "User Context"
Cohesion: 0.6
Nodes (3): Enumeration, Permission, RoleType

### Community 15 - "Keycloak Config"
Cohesion: 0.4
Nodes (3): PermissionAuthorizationHandler, AuthorizationHandler, IServiceProvider

### Community 16 - "Migrations"
Cohesion: 0.4
Nodes (3): PermissionAuthorizationPolicyProvider, AuthorizationOptions, DefaultAuthorizationPolicyProvider

### Community 17 - "DB Table Schema"
Cohesion: 0.4
Nodes (3): AuthDbContextModelSnapshot, AuthService.Infrastructure.Persistence.Migrations, ModelSnapshot

### Community 18 - "Authorization Handler"
Cohesion: 0.5
Nodes (3): UserContext, IHttpContextAccessor, IUserContext

## Knowledge Gaps
- **29 isolated node(s):** `IUserContext`, `IHttpContextAccessor`, `AuthDbContext`, `IDistributedCache`, `TimeSpan` (+24 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **21 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Infrastructure DependencyInjection` connect `Claims & Authorization` to `Keycloak Auth Service`?**
  _High betweenness centrality (0.011) - this node is a cross-community bridge._
- **What connects `IUserContext`, `IHttpContextAccessor`, `AuthDbContext` to the rest of the system?**
  _29 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Domain Value Objects` be split into smaller, more focused modules?**
  _Cohesion score 0.14 - nodes in this community are weakly interconnected._