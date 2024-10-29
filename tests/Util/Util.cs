﻿using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Util;

public class Util
{
    public static DateTime CurrentTime = DateTime.Now;

    public static void SeedData(ChirpDBContext context, int seed)
    {
        if (seed == 1)
        {
            Author author1 = new Author { Name = "TestUser1", Email = "Test1@exsample.dk", Cheeps = new List<Cheep>()};
            Author author2 = new Author { Name = "TestUser2", Email = "Test2@exsample.dk", Cheeps = new List<Cheep>()};
            // Seed data
            context.Authors.AddRange(author1, author2);

            context.Cheeps.AddRange(
                new Cheep { Text = "Hello World!", AuthorId = author1.Id, TimeStamp = CurrentTime, Author = author1},
                new Cheep { Text = "Another Cheep, hell yeah", AuthorId = author2.Id, TimeStamp = CurrentTime, Author = author2}
            );
        }
        else
        {
            var a1 = new Author() { Name = "Roger Histand", Email = "Roger+Histand@hotmail.com", Cheeps = new List<Cheep>() };
            
            context.Authors.AddRange(a1);
            
            for (int i = 0; i < 40; i++)
            {
                var cheep = new Cheep { Text = "" + i, AuthorId = a1.Id, TimeStamp = CurrentTime, Author = a1 };
                
                context.Cheeps.AddRange(cheep);
            }
        }
    }
    
    public static async Task<ChirpDBContext> CreateInMemoryDatabase(int seed)
    {
        //Create a new database in memory
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); 

        //pupulate the database
        SeedData(context, seed);

        await context.SaveChangesAsync();

        return context;
    }
}