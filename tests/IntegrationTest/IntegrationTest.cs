using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest;

public class IntegrationTest
{
 
 private DateTime CurrentTime = DateTime.Now;
    private async Task<ChirpDBContext> CreateInMemoryDatabase()
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


    [Fact]
    public async Task Test_GetCheeps()
    {
        //Initialize the database
           using var context = await CreateInMemoryDatabase();
           
           
            //Create the service
            ICheepService cheepService = new CheepService(new CheepRepository(context));
            var cheeps = await cheepService.GetCheeps(1);
        
            //Assert we have two and only two cheeps
            Assert.Equal(2, cheeps.Count);
    }
    
    
    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsFromAuthor(string author, string message)
    {
        //Initialize the database
           using var context = await CreateInMemoryDatabase();
           
           
            //Create the service
            ICheepService cheepService = new CheepService(new CheepRepository(context));
            var cheeps = await cheepService.GetCheeps(1);
            var cheepsFromAuthor = await cheepService.GetCheepsFromAuthor(1, author);

            //Assert that the data is correct
            Assert.Equal(message, cheepsFromAuthor.First().Message);
    }
    










    

    private async Task SeedData(ChirpDBContext context)
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

 
}