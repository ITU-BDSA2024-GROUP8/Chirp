using Chirp.Razor;
using Microsoft.Data.Sqlite;


namespace IntegrationTest;

public class IntegrationForDb

{

    private string DatabasePath = Path.Combine(Path.GetTempPath(), "chirp.db");

    [Theory]
    [InlineData("Helge", "Hello, BDSA students!")]
    public void create_db_and_read_data_from_author(string author, string message)
    {
        //Setup database
        DBFacade dbFacade = new DBFacade(DatabasePath);
        dbFacade.createDB();
        CheepService cheepService = new CheepService(dbFacade);
        var Equalmessage = cheepService.GetCheepsFromAuthor(author);

        //Assert that the database is created and that the data is read correctly
        Assert.Contains(message, Equalmessage.First().Message);
        
    }

    [Theory]
    [InlineData("nonexsisting")]
    public void create_db_and_read_data_from_nonexsisting_author(string author)
    {
        //Setup database
        DBFacade dbFacade = new DBFacade(DatabasePath);
        dbFacade.createDB();
        CheepService cheepService = new CheepService(dbFacade);
        var message = cheepService.GetCheepsFromAuthor(author);

        //Assert the message is empty from the nonexsisting author
        Assert.Empty(message);
    }


    [Fact]
    public void test_getCheeps()
    {
        //Setup database
        DBFacade dbFacade = new DBFacade(DatabasePath);
        dbFacade.createDB();
        CheepService cheepService = new CheepService(dbFacade);
        var cheeps = cheepService.GetCheeps();

        //Assert that the database is created and that the data is read correctly
        Assert.True(2 < cheeps.Count);

    }



    
}