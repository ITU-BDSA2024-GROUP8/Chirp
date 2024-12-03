using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");

builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ChirpDBContext>();

builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

var clientId = builder.Configuration["authentication_github_clientId"];
var clientSecret = builder.Configuration["authentication_github_clientSecret"];

if(clientId == null || clientSecret == null) throw new NullReferenceException("clientId or clientSecret is null");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie()
    .AddGitHub(o =>
    {
        o.ClientId = clientId;
        o.ClientSecret = clientSecret;
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//test
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//Custom redirect policy to the /Register and /Login page
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register") && (context.User.Identity?.IsAuthenticated ?? true))
    {
        context.Response.Redirect("/");
    }
    else if(context.Request.Path.StartsWithSegments("/Identity/Account/Login") && (context.User.Identity?.IsAuthenticated ?? true)) 
    {
        context.Response.Redirect("/");
    }
    else
    {
        await next();
    }
});

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetService<ChirpDBContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
    if (context == null) return;
    if(DbInitializer.CreateDb(context)) await DbInitializer.SeedDatabase(context, userManager);
}

app.Run();
