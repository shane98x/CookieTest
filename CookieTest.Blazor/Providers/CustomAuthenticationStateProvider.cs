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
using Microsoft.Extensions.Options;

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
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var client = httpClientFactory.CreateClient("API");

            try
            {
                var route = "Api/Auth/Info";
                var response = await client.GetAsync(route);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var claimsData = JsonSerializer.Deserialize<List<ClaimResult>>(responseContent, options);

                    var claimsIdentity = new ClaimsIdentity(nameof(CustomAuthenticationStateProvider));

                    foreach (var claim in claimsData)
                    {
                        claimsIdentity.AddClaim(new Claim(claim.Type, claim.Value));
                    }

                    user = new ClaimsPrincipal(claimsIdentity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new AuthenticationState(user);
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
                await store.ClearUserAsync();
                navigationManager.NavigateTo("/");
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
            else
            {
                // Logout failed
                await store.SaveUserAsync(true);
            }
        }
    }
}
