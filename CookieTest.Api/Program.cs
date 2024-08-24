using CookieTest.Api.Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string corsOrigins = "CorsOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Swagger doesn't support cookies in UI: https://swagger.io/docs/specification/authentication/cookie-authentication/

builder.Services.AddDbContext<CookieDbContext>(options =>
    options.UseInMemoryDatabase("CookieDb"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CookieTest";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Api/Auth/Login";
        options.LogoutPath = "/Api/Auth/Logout";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7234") 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed the database for both development and non-development environments
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CookieDbContext>();

    context.Database.EnsureCreated();

    context.Seed();
}


// Can set a dynamic policy like this later
//var cookiePolicyOptions = new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.Strict,
//};

//app.UseCookiePolicy(cookiePolicyOptions);

app.UseHttpsRedirection();

app.UseCors(corsOrigins);

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();

app.Run();
