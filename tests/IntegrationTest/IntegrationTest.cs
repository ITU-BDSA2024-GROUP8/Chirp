using Xunit;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Chirp.Repositories;

namespace IntegrationTest;

public class IntegrationTest : IAsyncLifetime  // Implementing IAsyncLifetime for async initialization
{
    private readonly ChirpDBContext _context;
    private readonly ICheepQueryRepository _cheepQueryRepository;
    private readonly ICheepCommandRepository _cheepCommandRepository;
    private readonly ICheepService _cheepService;

    public IntegrationTest()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChirpDBContext(options);
        _cheepQueryRepository = new CheepQueryRepository(_context);
        _cheepCommandRepository = new CheepCommandRepository(_context, _cheepQueryRepository);
        _cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);
    }

    // Implement IAsyncLifetime
    public async Task InitializeAsync()
    {
        // Set up initial test data
        var author = await _cheepCommandRepository.NewAuthorAsync("TestUser1", "Test1@exsample.dk");
        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, "Hello World!");
        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, "Second test cheep");
    }

    public Task DisposeAsync()
    {
        _context.Dispose();
        return Task.CompletedTask;
    }

    // Remove the InitializeTestEnvironment method since we're now using IAsyncLifetime

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {
        // Act
        var cheeps = await _cheepService.GetCheeps(1);
        var cheepsFromAuthor = await _cheepService.GetCheepsFromAuthor(1, author);

        // Assert
        Assert.Equal(2, cheeps.Count);
        Assert.Contains(cheepsFromAuthor, c => c.Message == message);
        Assert.All(cheepsFromAuthor, c => Assert.Equal(author, c.AuthorName));
    }

    // ... rest of your test methods remain the same, but remove the InitializeTestEnvironment calls ...
}