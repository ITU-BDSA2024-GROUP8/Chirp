using Chirp.Razor;
using Chirp.Razor.Data;

using Microsoft.EntityFrameworkCore;


namespace IntegrationTest;

public class IntegrationTest : IDisposable
{
    private string DatabasePath = Path.Combine(Path.GetTempPath(), "chirp_test.db");
    private readonly ChirpDBContext context;

    public IntegrationTest(){
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite($"Data Source={DatabasePath}") //Using SQLite with file path
            .Options;
        context = new ChirpDBContext(options);
    }


    [Theory]
    [InlineData("Helge", "Hello, BDSA students!")]
    public async void create_db_and_read_data_from_author(string author, string message)
    {
        //Ensure the database is created and initialized
            context.Database.EnsureCreated();
            await DbInitializer.CreateDb(context); //Populate the test data 

            //Query the data
            var cheeps = await context.Cheeps
                .Where(c => c.AuthorId == context.Authors.First(a => a.Name == author).AuthorId)
                .ToListAsync();

            //Assert that the data is correct
            Assert.Single(cheeps);
            Assert.Equal(message, cheeps.First().Text);
    }
    
    public void Dispose()
    {   
        context.Database.EnsureDeleted(); //Delete temp database
    }
}