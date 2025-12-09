using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Services;
using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IDateTimeProvider _timeProvider;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(
        IDateTimeProvider timeProvider, 
        IOptions<JwtSettings> jwtSettings)
    {
        _timeProvider = timeProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.FirstName + " " + user.LastName),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _timeProvider.UtcNow
                .AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials);

       return new JwtSecurityTokenHandler().WriteToken(securityToken);  
    }
}
