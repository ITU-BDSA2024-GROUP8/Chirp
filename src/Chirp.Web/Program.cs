using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetService<ChirpDBContext>();
    if (context != null)
    {
        if (DbInitializer.CreateDb(context)) 
        {
            DbInitializer.SeedDatabase(context);
        }
    }
}

// Specify the port and add the URL
var port = 5000; // Set your desired port here
app.Urls.Add($"http://localhost:{port}");

// Start the web server
var hostTask = Task.Run(() => app.Run()); // Start the application in a background task

// Open the default browser with the application's URL
var uri = $"http://localhost:{port}";
Process.Start(new ProcessStartInfo
{
    FileName = uri,
    UseShellExecute = true // This will use the OS default browser
});

// Wait for the host task to complete (if needed)
await hostTask; // Awaiting the host task will keep the application running
