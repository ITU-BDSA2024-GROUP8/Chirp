using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Util;

public class Util
{
    public static DateTime CurrentTime = DateTime.Now;

    public static async Task SeedData(ChirpDBContext context)
    {
        Author author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "Test1@exsample.dk", Cheeps = new List<Cheep>()};
        Author author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "Test2@exsample.dk", Cheeps = new List<Cheep>()};
        // Seed data
        context.Authors.AddRange(author1, author2);

        context.Cheeps.AddRange(
            new Cheep { CheepId = 1, Text = "Hello World!", AuthorId = 1, TimeStamp = CurrentTime, Author = author1},
            new Cheep { CheepId = 2, Text = "Another Cheep, hell yeah", AuthorId = 2, TimeStamp = CurrentTime, Author = author2}
        );
    }
    
    public static async Task<ChirpDBContext> CreateInMemoryDatabase()
    {
        //Create a new database in memory
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); 

        //pupulate the database
        await SeedData(context);

        await context.SaveChangesAsync();

        return context;
    }
}