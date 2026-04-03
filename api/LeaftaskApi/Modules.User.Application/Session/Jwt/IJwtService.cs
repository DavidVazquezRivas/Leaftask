using Modules.Users.Domain.Entities;

namespace Modules.Users.Application.Session.Jwt;

public interface IJwtService
{
    string GenerateToken(User user);
}
