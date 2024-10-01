using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using LMS.API.Service.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LMS.API.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration configuration;
    private User? user;

    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<TokenDto> CreateTokenAsync(bool expireTime)
    {
        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = await GetClaims();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims);

        ArgumentNullException.ThrowIfNull(user, nameof(user));

        user.RefreshToken = GenerateRefreshToken();

        if (expireTime)
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(2);

        var res = await userManager.UpdateAsync(user); //ToDo validate res!
        string accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, user.RefreshToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signing, IEnumerable<Claim> claims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var tokenOptions = new JwtSecurityToken(
                                    issuer: configuration["Issuer"],
                                    audience: configuration["Audience"],
                                    claims: claims,
                                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["Expires"])),
                                    signingCredentials: signing);

        return tokenOptions;
    }

    private async Task<IEnumerable<Claim>> GetClaims()
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>()
        {
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.NameIdentifier, user.Id!),
        new Claim(JwtRegisteredClaimNames.Aud, configuration["JwtSettings:Audience"]),
        new Claim(JwtRegisteredClaimNames.Iss, configuration["JwtSettings:Issuer"]),
         };

        // Add role claims
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        string? secretKey = configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));

        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration)
    {
        ArgumentNullException.ThrowIfNull(userForRegistration, nameof(userForRegistration));

        // Check if the provided role is either "Student" or "Teacher"
        var validRoles = new[] { "Student", "Teacher" };
        if (!validRoles.Contains(userForRegistration.Role))
        {
            // Return an error message if the role is not valid
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Invalid role: '{userForRegistration.Role}'. Only 'Student' or 'Teacher' roles are allowed."
            });
        }

        var user = new User
        {
            UserName = userForRegistration.UserName,
            Email = userForRegistration.Email,
            FirstName = userForRegistration.FirstName,
            LastName = userForRegistration.LastName,
            CourseId = userForRegistration.CourseId,
        };

        // Create the user
        IdentityResult result = await userManager.CreateAsync(user, userForRegistration.Password!);

        if (!result.Succeeded)
        {
            // If user creation fails, return the error
            return result;
        }

        // Assign the role to the user
        var roleResult = await userManager.AddToRoleAsync(user, userForRegistration.Role);
        if (!roleResult.Succeeded)
        {
            // If role assignment fails, delete the user and return the role-related errors
            await userManager.DeleteAsync(user);
            return IdentityResult.Failed(roleResult.Errors.ToArray());
        }

        return IdentityResult.Success;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto, nameof(userDto));

        user = await userManager.FindByNameAsync(userDto.UserName!);
        if (user == null) return false;

        return user != null && await userManager.CheckPasswordAsync(user, userDto.Password!);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);

        User? user = await userManager.FindByNameAsync(principal.Identity?.Name!);
        if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)

            //ToDo: Handle with middleware and custom exception class
            throw new ArgumentException("The TokenDto has som invalid values");

        // Generate new access token and refresh token
        this.user = user; // Set the user for the token creation
        user.RefreshToken = GenerateRefreshToken(); // Generate a new refresh token
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(2); // Update the expiration

        // Save the updated user to the database
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            // Handle update failure (throw or log errors)
            throw new Exception("Failed to update user with new refresh token");
        }

        // Create and return the new tokens
        return await CreateTokenAsync(expireTime: false);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");

        string? secretKey = configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(nameof(secretKey));

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };

        JwtSecurityTokenHandler tokenHandler = new();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}

