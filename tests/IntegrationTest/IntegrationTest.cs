namespace IntegrationTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Util;

public class IntegrationTest
{



    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();


        //Create the service
        ICheepService cheepService = new CheepService(new CheepRepository(context));
        var cheeps = await cheepService.GetCheeps(1);
        var cheepsFromAuthor = await cheepService.GetCheepsFromAuthor(1, author);


        //Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        //Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    //Test the GetCheepsFromAuthor method from CheepRepository

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepRepository(string author, string message)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();

        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var cheeps = await cheepRepository.GetCheepsAsync(1);
        var cheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author);

        //Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        //Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }



    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_GetAuthor(string author, string email)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var authorByName = await cheepRepository.GetAuthorByNameAsync(author);
        var authorByEmail = await cheepRepository.GetAuthorByEmailAsync(email);
        //Assert that the data is correct
        Assert.Equal(author, authorByName?.UserName);
        Assert.Equal(email, authorByEmail?.Email);
    }


    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_CreateCheep(string author, string email)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();

        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var cheeps = await cheepRepository.GetCheepsAsync(1);
        //Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);

        //Create a new cheep
        await cheepRepository.NewCheepAsync(author, "New Cheep!", email);
        cheeps = await cheepRepository.GetCheepsAsync(1);

        //Assert we have three and only three cheeps
        Assert.Equal(3, cheeps.Count);
    }

}