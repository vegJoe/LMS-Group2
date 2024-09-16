using LMS.API.Service.Contracts;

namespace LMS.API.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> _authService;
    public IAuthService AuthService => _authService.Value;

    public ServiceManager(Lazy<IAuthService> authService)
    {
        _authService = authService;
    }
}
