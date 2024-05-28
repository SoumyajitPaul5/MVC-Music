using Microsoft.EntityFrameworkCore;
using MVC_Music.Models;
using MVC_Music.ViewModels;
using System.Numerics;

namespace MVC_Music.Data
{
    public class MusicContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Property to get the current user's name
        public string UserName
        {
            get; private set;
        }

        // Constructor to initialize the context with options and HTTP context accessor
        public MusicContext(DbContextOptions<MusicContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                UserName = "Seed Data";
            }
        }

        // DbSets for Genre, Album, Song, and SongDetail models
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<MVC_Music.Models.SongDetail> SongDetail { get; set; }

        // Configures the model properties and relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Create unique index on Song title and AlbumID
            modelBuilder.Entity<Song>()
            .HasIndex(p => new { p.Title, p.AlbumID })
            .IsUnique();

            // Configure Genre to Album relationship
            modelBuilder.Entity<Genre>()
                .HasMany<Album>(p => p.Albums)
                .WithOne(c => c.Genre)
                .HasForeignKey(c => c.GenreID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Genre to Song relationship
            modelBuilder.Entity<Genre>()
                .HasMany<Song>(p => p.Songs)
                .WithOne(c => c.Genre)
                .HasForeignKey(c => c.GenreID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Album to Song relationship
            modelBuilder.Entity<Album>()
                .HasMany<Song>(p => p.Songs)
                .WithOne(c => c.Album)
                .HasForeignKey(c => c.AlbumID)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Override SaveChanges to include auditing information
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        // Override SaveChangesAsync to include auditing information
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        // Method to set auditing information before saving changes
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
