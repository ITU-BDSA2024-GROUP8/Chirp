using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> db = CSVDatabase<Cheep>.Instance;

app.MapGet("/", () => "Hello World!");

app.MapGet("/cheeps", () =>
{
    IEnumerable<Cheep> cheeps = db.Read(); 
    return cheeps;
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    db.Store(cheep);
    return Results.StatusCode(200);
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);