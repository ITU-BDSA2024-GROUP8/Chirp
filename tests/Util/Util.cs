using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Util;

public class Util
{
    public static DateTime CurrentTime = DateTime.Now;

    public static void SeedData(ChirpDBContext context)
    {
        Author author1 = new Author { UserName = "TestUser1", Email = "Test1@exsample.dk", Cheeps = new List<Cheep>()};
        Author author2 = new Author { UserName = "TestUser2", Email = "Test2@exsample.dk", Cheeps = new List<Cheep>()};
        // Seed data
        context.Authors.AddRange(author1, author2);

        context.Cheeps.AddRange(
            new Cheep { CheepId = 1, Text = "Hello World!", AuthorId = author1.Id, TimeStamp = CurrentTime, Author = author1},
            new Cheep { CheepId = 2, Text = "Another Cheep, hell yeah", AuthorId = author2.Id, TimeStamp = CurrentTime, Author = author2}
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
        SeedData(context);

        await context.SaveChangesAsync();

        return context;
    }
}