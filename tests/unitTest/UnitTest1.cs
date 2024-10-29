using Chirp.Infrastructure.Models;

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
        using var context = await Util.CreateInMemoryDatabase(1);
           
           
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var authorByName = await cheepRepository.GetAuthorByNameAsync(Author);

        Assert.Equal(Author, authorByName?.Name);
    }
    
    [Theory]
    [InlineData("Test1@exsample.dk")]
    public async Task Test_FindAuthorByEmail(string Email)
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var authorByEmail = await cheepRepository.GetAuthorByEmailAsync(Email);
        
        Assert.Equal(Email, authorByEmail?.Email);
    }
    
    
    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        var author = await cheepRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");
        var authorByName = await cheepRepository.GetAuthorByNameAsync(author.Name);
        
        Assert.Equal(author, authorByName);
    }
    
    [Fact]
    public async Task Test_CreateNewCheep()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);
        
        var author = await cheepRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");

        var cheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author.Name);

        Assert.Equal(0, cheepsFromAuthor.Count);
        
        await cheepRepository.NewCheepAsync(author.Name, author.Email, "This is a new test cheep");
        
        var newCheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author.Name);
        
        Assert.Equal(1, newCheepsFromAuthor.Count);
    }
    
    [Fact]
    public async Task Test_CheepsForACertainPage()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase(2);

        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);

        var cheepsOnPage1 = await cheepRepository.GetCheepsAsync(1);
        var cheepsOnPage2 = await cheepRepository.GetCheepsAsync(2);

        Assert.Equal(32, cheepsOnPage1.Count);
        Assert.Equal(8, cheepsOnPage2.Count);
    }
    
    [Fact]
    public async Task Test_CheepsForACertainPageByAuthor()
    {
        //Initialize the database
        using var context = await Util.CreateInMemoryDatabase(2);

        //Create the service
        ICheepRepository cheepRepository = new CheepRepository(context);

        var author = await cheepRepository.GetAuthorByNameAsync("Roger Histand");
        var cheepsOnPage = await cheepRepository.GetCheepsFromAuthorAsync(1, author.Name);

        foreach (var Cheep in cheepsOnPage)
        {
            Assert.Equal(Cheep.Author, author.Name);
        }
    }
}