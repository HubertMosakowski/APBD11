using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APBD11Login.Context;
using APBD11Login.Helpers;
using APBD11Login.Models;
using APBD11Login.Models.AuthModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace APBD11Login.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DbaseContext _context;
    
    public UserController(IConfiguration configuration, DbaseContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult RegisterUser(RegisterRequest registerRequest)
    {
        var hashedPasswordAndSalt = SecHelpers.GetHashedPasswordAndSalt(registerRequest.Password);
        
        var user = new AppUser()
        {
            Email = registerRequest.Email,
            Login = registerRequest.Login,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecHelpers.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {
        AppUser user = _context.Users.Where(u => u.Login == loginRequest.Login).FirstOrDefault();

        string passHashFromDb = user.Password;
        string currentHashedPassword = SecHelpers.GetHashedPasswordWithSalt(loginRequest.Password, user.Salt);

        if (passHashFromDb != currentHashedPassword)
        {
            return Unauthorized();
        }


        Claim[] userClaim = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "user"),
            new Claim(ClaimTypes.Role, "admin")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: userClaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        user.RefreshToken = SecHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        _context.SaveChanges();

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = user.RefreshToken
        });
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenRequest refreshToken)
    {
        AppUser user = _context.Users.Where(u => u.RefreshToken == refreshToken.RefreshToken).FirstOrDefault();
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExp < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }
        
        Claim[] userClaim = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "user"),
            new Claim(ClaimTypes.Role, "admin")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: userClaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: credentials
        );

        user.RefreshToken = SecHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        _context.SaveChanges();

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            refreshToken = user.RefreshToken
        });
    }
}