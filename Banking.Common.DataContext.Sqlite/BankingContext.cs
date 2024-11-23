using Banking.Shared;
using Microsoft.EntityFrameworkCore;

namespace Banking.Shared
{
    public class BankingContext : DbContext
    {
        public BankingContext() { }

        public BankingContext(DbContextOptions<BankingContext> options)
            : base(options) { }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Filename=../Banking.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountNumber);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Balance).IsRequired();
                entity.Property(a => a.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity
                    .HasOne(t => t.FromAccount)
                    .WithMany()
                    .HasForeignKey(t => t.FromAccountNumber)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(t => t.ToAccount)
                    .WithMany()
                    .HasForeignKey(t => t.ToAccountNumber)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.Type).IsRequired();

                entity.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18, 2)");

                entity.Property(t => t.Date).IsRequired();
            });
        }
    }
}
