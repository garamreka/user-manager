using Microsoft.Extensions.Options;
using UserManager.Api.Configurations;

namespace UserManager.Api.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IOptions<AuthenticationConfiguration> configuration;

        public AuthenticationService(IOptions<AuthenticationConfiguration> configuration)
        {
            this.configuration = configuration;
        }

        public Task<AuthUser> Authenticate(string username, string password)
        {
            foreach (var userData in configuration.Value.Users)
            {
                if (userData.UserName == username && userData.Password == password)
                {
                    var authUser = new AuthUser
                    {
                        Username = username,
                        Id = Guid.NewGuid().ToString("N")
                    };

                    return Task.FromResult(authUser);
                }
            }

            return Task.FromResult<AuthUser>(null);
        }
    }
}
