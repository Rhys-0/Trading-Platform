using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TradingApp.Data.Interfaces;

namespace TradingApp.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthenticationService _authenticationService;

        public CustomAuthenticationStateProvider(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            if (_authenticationService.IsAuthenticated && _authenticationService.CurrentUser != null)
            {
                var user = _authenticationService.CurrentUser;
                
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim("UserId", user.Id.ToString())
                }, "customAuth");

                Console.WriteLine($"?? AUTH STATE: User authenticated - {user.Username}");
            }
            else
            {
                Console.WriteLine("?? AUTH STATE: No authenticated user");
            }

            var claimsPrincipal = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public void NotifyAuthenticationStateChanged()
        {
            Console.WriteLine("?? AUTH STATE: Notifying authentication state changed");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}