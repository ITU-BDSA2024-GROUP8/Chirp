namespace IntegrationTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Util;

public class IntegrationTest
{
    [Fact]
    public async Task Test_GetCheepsUsingCheepService()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
           
        //Create the service
        ICheepService cheepService = new CheepService(new CheepRepository(context));
        var cheeps = await cheepService.GetCheeps(1);
    
        //Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
    }
    
    
    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsFromAuthorUsingCheepService(string author, string message)
    {
        //Initialize the database
           using var context = await Util.CreateInMemoryDatabase();
           
           
            //Create the service
            ICheepService cheepService = new CheepService(new CheepRepository(context));
            var cheeps = await cheepService.GetCheeps(1);
            var cheepsFromAuthor = await cheepService.GetCheepsFromAuthor(1, author);

            //Assert that the data is correct
            Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

       [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsFromAuthorUsingCheepRepository(string author, string message)
    {
        //Initialize the database
           using var context = await Util.CreateInMemoryDatabase();
           
            //Create the service
            ICheepRepository cheepRepository = new CheepRepository(context);
            var cheeps = await cheepRepository.GetCheepsAsync(1);
            var cheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author);

            //Assert that the data is correct
            Assert.Equal(message, cheepsFromAuthor.First().Message);
    }
}