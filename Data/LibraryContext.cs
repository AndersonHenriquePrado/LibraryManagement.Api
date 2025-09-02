using Microsoft.EntityFrameworkCore;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }
        
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Genre entity
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });
            
            // Configure Author entity
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Biography).HasMaxLength(1000);
                entity.Property(e => e.Nationality).HasMaxLength(100);
                entity.Ignore(e => e.FullName); // Computed property
            });
            
            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Publisher).HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                
                // Configure relationships
                entity.HasOne(e => e.Author)
                      .WithMany(a => a.Books)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(e => e.Genre)
                      .WithMany(g => g.Books)
                      .HasForeignKey(e => e.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasIndex(e => e.ISBN).IsUnique();
            });
            
            // Seed data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Ficção", Description = "Obras de ficção literária", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Genre { Id = 2, Name = "Romance", Description = "Histórias românticas", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Genre { Id = 3, Name = "Mistério", Description = "Livros de mistério e suspense", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Genre { Id = 4, Name = "Fantasia", Description = "Obras de fantasia e ficção científica", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Genre { Id = 5, Name = "Biografia", Description = "Biografias e autobiografias", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            // Seed Authors
            var utcNow = DateTime.UtcNow;
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Machado", LastName = "de Assis", Biography = "Escritor brasileiro, considerado um dos maiores nomes da literatura nacional.", BirthDate = new DateTime(1839, 6, 21, 0, 0, 0, DateTimeKind.Utc), DeathDate = new DateTime(1908, 9, 29, 0, 0, 0, DateTimeKind.Utc), Nationality = "Brasileira", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Author { Id = 2, FirstName = "Clarice", LastName = "Lispector", Biography = "Escritora brasileira nascida na Ucrânia, uma das principais representantes da literatura brasileira.", BirthDate = new DateTime(1920, 12, 10, 0, 0, 0, DateTimeKind.Utc), DeathDate = new DateTime(1977, 12, 9, 0, 0, 0, DateTimeKind.Utc), Nationality = "Brasileira", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Author { Id = 3, FirstName = "Jorge", LastName = "Amado", Biography = "Escritor brasileiro, um dos autores mais adaptados para cinema, teatro e televisão.", BirthDate = new DateTime(1912, 8, 10, 0, 0, 0, DateTimeKind.Utc), DeathDate = new DateTime(2001, 8, 6, 0, 0, 0, DateTimeKind.Utc), Nationality = "Brasileira", CreatedAt = utcNow, UpdatedAt = utcNow }
            );
            
            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Dom Casmurro", ISBN = "9788525406958", Description = "Romance clássico da literatura brasileira", PublicationDate = new DateTime(1899, 1, 1, 0, 0, 0, DateTimeKind.Utc), Publisher = "Globo", Pages = 208, Price = 29.90m, AuthorId = 1, GenreId = 1, CreatedAt = utcNow, UpdatedAt = utcNow },
                new Book { Id = 2, Title = "A Hora da Estrela", ISBN = "9788520925188", Description = "Último romance de Clarice Lispector", PublicationDate = new DateTime(1977, 1, 1, 0, 0, 0, DateTimeKind.Utc), Publisher = "Rocco", Pages = 87, Price = 24.90m, AuthorId = 2, GenreId = 1, CreatedAt = utcNow, UpdatedAt = utcNow },
                new Book { Id = 3, Title = "Gabriela, Cravo e Canela", ISBN = "9788535909814", Description = "Romance de Jorge Amado ambientado em Ilhéus", PublicationDate = new DateTime(1958, 1, 1, 0, 0, 0, DateTimeKind.Utc), Publisher = "Companhia das Letras", Pages = 424, Price = 39.90m, AuthorId = 3, GenreId = 2, CreatedAt = utcNow, UpdatedAt = utcNow }
            );
        }
        
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Genre || e.Entity is Author || e.Entity is Book)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
                
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}

