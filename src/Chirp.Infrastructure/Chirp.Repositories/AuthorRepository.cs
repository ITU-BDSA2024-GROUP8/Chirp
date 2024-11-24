using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

    public interface IAuthorRepository
    {
        public Task<Author?> GetAuthorByNameAsync(string name);
        public Task<Author?> GetAuthorByEmailAsync(string email);
        public  Task<Author> NewAuthorAsync(string authorName, string authorEmail);
        public  Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId);
        public  Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId);
        public  Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId);
    }
     public class AuthorRepository : IAuthorRepository
    {
        private readonly ChirpDBContext _dbContext;

        public AuthorRepository(ChirpDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Author?> GetAuthorByNameAsync(string name)
        {
            var query = from a in _dbContext.Authors
                        where a.Name == name
                        select a;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Author?> GetAuthorByEmailAsync(string email)
        {
            var query = from a in _dbContext.Authors
                        where a.Email == email
                        select a;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Author> NewAuthorAsync(string authorName, string authorEmail)
        {
            var newAuthor = new Author
            {
                Name = authorName,
                Email = authorEmail,
                Cheeps = new List<Cheep>(),
                Followers = new List<AuthorFollower>(),
                Following = new List<AuthorFollower>()
            };

            _dbContext.Authors.Add(newAuthor);
            await _dbContext.SaveChangesAsync();

            return newAuthor;
        }

        public async Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId)
        {
            var followRelation = new AuthorFollower
            {
                FollowerId = currentAuthorId,
                FollowingId = targetAuthorId
            };

            _dbContext.AuthorFollowers.Add(followRelation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId)
        {
            var followRelation = await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
                a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);

            _dbContext.AuthorFollowers.Remove(followRelation!);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId)
        {
            var followRelation = await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
                a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);

            return followRelation != null;
        }
    }