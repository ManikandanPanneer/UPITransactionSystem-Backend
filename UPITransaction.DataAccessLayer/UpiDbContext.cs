using Microsoft.EntityFrameworkCore;
using UPITransaction.DataAccessLayer.Entities;

namespace UPITransaction.DataAccessLayer
{
    public class UpiDbContext : DbContext
    {
        public UpiDbContext(DbContextOptions<UpiDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // To make Phone Number unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Sender relationship with Transaction (delete behavior ) for cascading
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany(u => u.SentTransactions)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver relationship with Transaction (delete behavior ) for cascading
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Receiver)
                .WithMany(u => u.ReceivedTransactions)
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
