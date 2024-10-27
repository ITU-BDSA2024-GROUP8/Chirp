namespace unitTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Util;

public class UnitTest1
{
    [Theory]
    [InlineData("TestUser1")]
    public async Task Test_FindAuthorByName(string Author)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
           
           
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var authorByName = await cheepRepository.GetAuthorByNameAsync(Author);

        Assert.Equal(Author, authorByName?.UserName);
    }
    
    [Theory]
    [InlineData("Test1@exsample.dk")]
    public async Task Test_FindAuthorByEmail(string Email)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var authorByEmail = await cheepRepository.GetAuthorByEmailAsync(Email);
        
        Assert.Equal(Email, authorByEmail?.Email);
    }
    
    
    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var author = await cheepRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");
        var authorByName = await cheepRepository.GetAuthorByNameAsync(author.UserName);
        
        Assert.Equal(author, authorByName);
    }
    
    [Fact]
    public async Task Test_CreateNewCheep()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase();
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        
        var author = await cheepRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");

        var cheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author.UserName);

        Assert.Equal(0, cheepsFromAuthor.Count);
        
        await cheepRepository.NewCheepAsync(author.UserName, author.Email, "This is a new test cheep");
        
        var newCheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author.UserName);
        
        Assert.Equal(1, newCheepsFromAuthor.Count);
    }
}