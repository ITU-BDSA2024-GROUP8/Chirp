using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure;

public static class DbInitializer
{
    public static bool CreateDb(ChirpDBContext context)
    {
        return context.Database.EnsureCreated();
    }
    
    public static void SeedDatabase(ChirpDBContext chirpContext)
    {
        if (!(chirpContext.Authors.Any() && chirpContext.Cheeps.Any()))
        {
            var a1 = new Author() { UserName = "Roger Histand", Email = "Roger+Histand@hotmail.com", Cheeps = new List<Cheep>() };
    
            var authors = new List<Author>() { a1 };

            var c1 = new Cheep() { CheepId = 1, AuthorId = a1.Id, Author = a1, Text = "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
            
            var cheeps = new List<Cheep>() { c1 };
            a1.Cheeps = new List<Cheep>() { c1 };
    
            chirpContext.Authors.AddRange(authors);
            chirpContext.Cheeps.AddRange(cheeps);
            chirpContext.SaveChanges();
        }
    }
}