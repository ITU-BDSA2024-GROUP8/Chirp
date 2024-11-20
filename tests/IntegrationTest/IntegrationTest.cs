namespace IntegrationTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Util;
using Xunit;

public class IntegrationTest
{
    private ICheepQueryRepository _cheepQueryRepository;
    private ICheepCommandRepository _cheepCommandRepository;
    private ICheepService _cheepService;

    public IntegrationTest()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: "ChirpTestDb")
            .Options;

        var context = new ChirpDBContext(options);

        _cheepQueryRepository = new CheepQueryRepository(context);
        _cheepCommandRepository = new CheepCommandRepository(context, _cheepQueryRepository);
        _cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        ICheepService cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);
        var cheeps = await cheepService.GetCheeps(1);
        var cheepsFromAuthor = await cheepService.GetCheepsFromAuthor(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepRepository(string author, string message)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        ICheepQueryRepository cheepQueryRepository = new CheepQueryRepository(context);
        var cheeps = await cheepQueryRepository.GetCheepsAsync(1);
        var cheepsFromAuthor = await cheepQueryRepository.GetCheepsFromAuthorAsync(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_GetAuthor(string author, string email)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        ICheepQueryRepository cheepQueryRepository = new CheepQueryRepository(context);
        var authorByName = await cheepQueryRepository.GetAuthorByNameAsync(author);
        var authorByEmail = await cheepQueryRepository.GetAuthorByEmailAsync(email);

        // Assert that the data is correct
        Assert.Equal(author, authorByName?.Name);
        Assert.Equal(email, authorByEmail?.Email);
    }

    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_CreateCheep(string author, string email)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        ICheepCommandRepository cheepCommandRepository = new CheepCommandRepository(context, _cheepQueryRepository);
        var cheeps = await _cheepQueryRepository.GetCheepsAsync(1);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);

        // Create a new cheep
        await cheepCommandRepository.NewCheepAsync(author, email, "New Cheep!");
        cheeps = await _cheepQueryRepository.GetCheepsAsync(1);

        // Assert we have three and only three cheeps
        Assert.Equal(3, cheeps.Count);
    }
}