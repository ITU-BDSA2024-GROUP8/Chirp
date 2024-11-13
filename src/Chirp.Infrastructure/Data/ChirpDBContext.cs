using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Data;

public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorFollower> AuthorFollowers { get; set; }
    
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AuthorFollower>()
            .HasKey(e => new { e.FollowerId, e.FollowingId });
    
        modelBuilder.Entity<AuthorFollower>()
            .HasOne(e => e.Follower)
            .WithMany(e => e.Following)
            .HasForeignKey(e => e.FollowerId);
    
        modelBuilder.Entity<AuthorFollower>()
            .HasOne(e => e.Following)
            .WithMany(e => e.Followers)
            .HasForeignKey(e => e.FollowingId);
    }
}