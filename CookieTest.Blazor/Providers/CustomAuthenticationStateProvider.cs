using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using CookieTest.Blazor.Models;
using Microsoft.AspNetCore.Components;
using static CookieTest.Blazor.Pages.Home;

namespace CookieTest.Blazor.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly NavigationManager navigationManager;
        private bool isUserAuthenticated = false; 

        public CustomAuthenticationStateProvider(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            this.httpClientFactory = httpClientFactory;
            this.navigationManager = navigationManager;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = isUserAuthenticated
                ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "OK") }, nameof(CustomAuthenticationStateProvider))
                : new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public async Task LoginAsync(LoginModel model)
        {
            var client = httpClientFactory.CreateClient("API");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Api/Auth/Login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                isUserAuthenticated = true;
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                navigationManager.NavigateTo("/User"); 
            }
            else
            {
                isUserAuthenticated = false;
            }
        }

        public async Task LogoutAsync()
        {
            var client = httpClientFactory.CreateClient("API");
            var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Api/Auth/Logout", emptyContent);

            if (response.IsSuccessStatusCode)
            {
                isUserAuthenticated = false;
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                navigationManager.NavigateTo("/"); 
            }
            else
            {
                isUserAuthenticated = true;
            }
        }
    }
}
