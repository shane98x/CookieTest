using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using CookieTest.Blazor;
using CookieTest.Blazor.Handler;
using Microsoft.AspNetCore.Components.Authorization;
using CookieTest.Blazor.Providers;
using Blazored.LocalStorage;
using CookieTest.Blazor.Store;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to use the API base address
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7078") });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<UserStore>();

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddHttpClient("API", options => {
    options.BaseAddress = new Uri("https://localhost:5092");
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

await builder.Build().RunAsync();
