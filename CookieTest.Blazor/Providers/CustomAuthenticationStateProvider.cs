using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using CookieTest.Blazor.Models;
using Microsoft.AspNetCore.Components;
using static CookieTest.Blazor.Pages.Home;
using CookieTest.Blazor.Store;

namespace CookieTest.Blazor.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly NavigationManager navigationManager;
        private readonly UserStore store;

        public CustomAuthenticationStateProvider(IHttpClientFactory httpClientFactory, NavigationManager navigationManager, UserStore store)
        {
            this.httpClientFactory = httpClientFactory;
            this.navigationManager = navigationManager;
            this.store = store;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await store.GetUserAsync();

            var identity = user
                ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "OK") }, nameof(CustomAuthenticationStateProvider))
                : new ClaimsIdentity();

            var userClaims = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(userClaims));
        }

        public async Task LoginAsync(LoginModel model)
        {
            var client = httpClientFactory.CreateClient("API");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Api/Auth/Login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                await store.SaveUserAsync(true);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                navigationManager.NavigateTo("/User");
            }
            else
            { 
                await store.SaveUserAsync(false);
            }
        }

        public async Task LogoutAsync()
        {
            var client = httpClientFactory.CreateClient("API");
            var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Api/Auth/Logout", emptyContent);

            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                await store.ClearUserAsync();
                navigationManager.NavigateTo("/"); 
            }
            else
            {
                // Logout failed
                await store.SaveUserAsync(true);
            }
        }
    }
}
