using DataAccess.Entities;

namespace Security.Services.Authorization;

public interface IAuthorizationService
{
    public string GenerateToken(User user);
}