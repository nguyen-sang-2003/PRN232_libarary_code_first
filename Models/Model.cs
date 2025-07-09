using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Author
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

}


public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual List<Book> Books { get; set; } = [];

}

public class BookCategory
{
    [ForeignKey("Book")]
    public int BookId { get; set; }
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public virtual Book Book { get; set; } = null;
    public virtual Category Category { get; set; } = null;
}

public class Book
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public string ImageBase64 { get; set; } // in database
    //public string ImageUrl { get; set; } // base64 png
    // image 2-4mb bytes => string/base64 png
    public DateTime PublishedDate { get; set; }
    public virtual List<BookCopy> BookCopies { get; set; }

    public virtual List<Category> Categories { get; set; } = [];
    public virtual Author Author { get; set; }
}

public class BookCopy
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Book")]
    public int BookId { get; set; }
    // dang muon, unavailable, available
    public string Status { get; set; } // "available"
    public string Condition { get; set; } // "new", "old", "mid"
    // rental
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Book Book { get; set; }
}

public class Rental
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    [ForeignKey("BookCopy")]
    public int BookCopyId { get; set; }
    public string Status { get; set; }
    public int RenewCount { get; set; } // count number renew book
    public DateTime RentalDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; }
    public virtual BookCopy Book { get; set; }
}

public class Return
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Rental")]
    public int RentalId { get; set; }
    public string Condition { get; set; }
    public DateTime ReturnDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Rental Rental { get; set; }
}

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string? PasswordResetToken { get; set; } // for password reset
    public long? PasswordResetTokenExpiryUnixTS { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; }
    // edit website/admin
}
public class Rule
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PrnContext : DbContext
{
    public PrnContext()
    {

    }

    public PrnContext(DbContextOptions<PrnContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("Default");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookCategory> BookCategory { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Return> Returns { get; set; }
    public DbSet<Rule> Rules { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BookCategory>().HasKey(bc => new { bc.BookId, bc.CategoryId });
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "George Orwell" },
            new Author { Id = 2, Name = "Harper Lee" },
            new Author { Id = 3, Name = "F. Scott Fitzgerald" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Classic" },
            new Category { Id = 2, Name = "Dystopian" },
            new Category { Id = 3, Name = "Drama" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "1984", AuthorId = 1, PublishedDate = new DateTime(1949, 6, 8), ImageBase64 = "" },
            new Book { Id = 2, Title = "To Kill a Mockingbird", AuthorId = 2, PublishedDate = new DateTime(1960, 7, 11), ImageBase64 = "" },
            new Book { Id = 3, Title = "The Great Gatsby", AuthorId = 3, PublishedDate = new DateTime(1925, 4, 10), ImageBase64 = "" }
        );

        modelBuilder.Entity<BookCopy>().HasData(
            new BookCopy { Id = 1, BookId = 1, Status = "available", Condition = "new", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new BookCopy { Id = 2, BookId = 1, Status = "unavailable", Condition = "old", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new BookCopy { Id = 3, BookId = 2, Status = "available", Condition = "mid", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new BookCopy { Id = 4, BookId = 3, Status = "available", Condition = "new", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "admin123", Active = true, Role = "admin", Email = "admin@example.com" },
            new User { Id = 2, Username = "john_doe", Password = "password", Active = true, Role = "user", Email = "john_doe@example.com" },
            new User { Id = 3, Username = "librarian", Password = "librarypass", Active = true, Role = "staff", Email = "staff1@example.com" }
        );

        modelBuilder.Entity<BookCategory>().HasData(
            new BookCategory { BookId = 1, CategoryId = 2 }, // 1984 - Dystopian
            new BookCategory { BookId = 2, CategoryId = 3 }, // To Kill a Mockingbird - Drama
            new BookCategory { BookId = 3, CategoryId = 1 }  // Gatsby - Classic
        );
        modelBuilder.Entity<Rule>().HasData(
            new Rule { Id = 1, Title = "rule 1", Content = "none", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        );

        modelBuilder.Entity<Rental>().HasData(
            new Rental { Id = 1, UserId = 2, BookCopyId = 1, Status = "borrowed", RenewCount = 1, RentalDate = DateTime.Now, DueDate = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        );

        modelBuilder.Entity<Return>().HasData(
            new Return { Id = 1, RentalId = 1, Condition = "100%", ReturnDate = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        );
    }
}
