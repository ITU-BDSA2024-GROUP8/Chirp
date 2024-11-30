using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Util;

public class Util
{
    public static DateTime CurrentTime = DateTime.Now;

    public static void SeedData(ChirpDBContext context, int seed)
    {
        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };
        Achievement ach5 = new Achievement() { Title = "Night Owl", Description = "Dark mode explored. Who needs sleep when you can cheep in the shadows?", ImagePath = "/images/Badges/activate-dark-mode-badge.png" };

        context.Achievements.AddRange(ach1, ach2, ach3, ach4, ach5);
        
        if (seed == 1)
        {
            Author author1 = new Author { 
                Name = "TestUser1", 
                Email = "test1@example.dk", 
                Cheeps = new List<Cheep>(), 
                Followers = new List<AuthorFollower>(), 
                Following = new List<AuthorFollower>()
            };
            
            Author author2 = new Author { 
                Name = "TestUser2", 
                Email = "test2@example.dk", 
                Cheeps = new List<Cheep>(), 
                Followers = new List<AuthorFollower>(), 
                Following = new List<AuthorFollower>()
            };
            
            // Seed data
            context.Authors.AddRange(author1, author2);

            context.Cheeps.AddRange(
                new Cheep { 
                    Text = "Hello World!", 
                    AuthorId = author1.Id, 
                    TimeStamp = CurrentTime,
                    Author = author1
                },
                new Cheep { 
                    Text = "Another Cheep, hell yeah", 
                    AuthorId = author2.Id, 
                    TimeStamp = CurrentTime,
                    Author = author2
                }
            );
        }
        else
        {
            var a1 = new Author() { 
                Name = "Roger Histand", 
                Email = "Roger+Histand@hotmail.com", 
                Cheeps = new List<Cheep>(), 
                Followers = new List<AuthorFollower>(), 
                Following = new List<AuthorFollower>() 
            };
            
            context.Authors.AddRange(a1);
            
            for (int i = 0; i < 40; i++)
            {
                var cheep = new Cheep { 
                    Text = "" + i, 
                    AuthorId = a1.Id, 
                    TimeStamp = CurrentTime,
                    Author = a1
                };
                
                context.Cheeps.AddRange(cheep);
            }
        }
    }
    
    public static async Task<ChirpDBContext> CreateInMemoryDatabase(int seed)
    {
        // Create a new database in memory
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); 

        // Populate the database
        SeedData(context, seed);

        await context.SaveChangesAsync();

        return context;
    }
}