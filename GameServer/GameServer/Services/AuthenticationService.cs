using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameServer.Db;
using Microsoft.IdentityModel.Tokens;

namespace GameServer.Services;

public class AuthenticationService : IAuthenticationService
{
    private Settings _settings;
    private GameDbContext _context;

    public AuthenticationService(Settings settings, GameDbContext context)
    {
        _context = context;
        _settings = settings;
    }

    public (bool success, string content) Register(string username, string password)
    {
        if (_context.Users.Any(u => u.UserName == username))
            return (false, "Username not available");

        var user = new User {UserName = username, PasswordHash = password};
        user.ProvideSaltAndHash();

        _context.Add(user);
        _context.SaveChanges();

        return (true, "");
    }

    public (bool success, string token) Login(string username, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);
        if (user == null)
            return (false, "Invalid Username");

        if (user.PasswordHash != AuthenticationHelper.ComputeHash(password, user.Salt))
            return (false, "Invalid password");

        return (true, GenerateJwtToken(AssembleClaimsIdentity(user)));
    }

    private ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim("id", user.Id.ToString()),
        });

        return subject;
    }

    private string GenerateJwtToken(ClaimsIdentity subject)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.BearerKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = DateTime.Now.AddYears(10),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}