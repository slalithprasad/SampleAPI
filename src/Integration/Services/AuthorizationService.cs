using Integration.Interfaces;

namespace Integration.Services;

public class AuthorizationService : IAuthorizationService
{
    public bool ValidateToken()
    {
        return true;
    }
}
