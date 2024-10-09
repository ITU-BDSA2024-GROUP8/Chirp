using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();

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

app.UseRouting();

app.MapRazorPages();

if(!DbInitializer.DbExists(dbPath)) await DbInitializer.CreateDb(app);

app.Run();