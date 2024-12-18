using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Chirp.Services;

public interface IAuthorService
{
    public Task<AuthorDTO?> GetAuthorByNameAsync(string name);
    public Task<AuthorDTO?> GetAuthorByEmailAsync(string email);
    public Task FollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task UnfollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId);
    public Task<List<string>> GetFollowingAsync(string authorId);
    public Task<List<string>> GetFollowersAsync(string authorId);
    public Task<string?> UpdateBioAsync(string authorId, string? newBio);
}

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    
    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }
    
    public async Task<AuthorDTO?> GetAuthorByNameAsync(string name)
    {
        return await _authorRepository.GetAuthorByNameAsync(name);
    }
    
    public async Task<AuthorDTO?> GetAuthorByEmailAsync(string email)
    {
        return await _authorRepository.GetAuthorByEmailAsync(email);
    }
    
    public async Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        await _authorRepository.FollowAuthorAsync(currentAuthorId, targetAuthorId);
    }
    
    public async Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        await _authorRepository.UnfollowAuthorAsync(currentAuthorId, targetAuthorId);
    }

    public async Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId)
    {
        return await _authorRepository.IsFollowingAsync(currentAuthorId, targetAuthorId);
    }
    
    public async Task<List<string>> GetFollowingAsync(string authorId){
        return await _authorRepository.GetFollowingAsync(authorId);
    }
    
    public async Task<List<string>> GetFollowersAsync(string authorId){
        return await _authorRepository.GetFollowersAsync(authorId);
    }
    
    public async Task<string?> UpdateBioAsync(string authorId, string? newBio)
    {
        return await _authorRepository.UpdateBioAsync(authorId, newBio);
    }
}