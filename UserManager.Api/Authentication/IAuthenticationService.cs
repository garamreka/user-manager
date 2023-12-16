namespace UserManager.Api.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthUser> Authenticate(string username, string password);
    }
}
