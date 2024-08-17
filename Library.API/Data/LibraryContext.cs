using Library.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Borrower)
                .WithMany(u => u.BorrowedBooks)
                .HasForeignKey(b => b.BorrowerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Book>()
                .Property(b => b.BorrowerId)
                .IsRequired(false);
        }
    }
}