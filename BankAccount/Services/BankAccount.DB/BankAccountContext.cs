using BankAccount.Domain;
using BankAccount.Domain.Entities.BankAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BankAccount.DB
{
    public class BankAccountContext : DbContext
    {
        public DbSet<DbAccount>? Account { get; set; }
        public DbSet<DbTransaction>? Transaction { get; set; }
        
        public BankAccountContext(DbContextOptions<BankAccountContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbAccount>(account =>
            {
                account.HasKey(ac => ac.Id);
                account.HasMany(ac => ac.Transactions).WithOne(t => t.Account);
                account.HasIndex(ac => ac.Reference).IsUnique(true);
            });
            modelBuilder.Entity<DbTransaction>(transaction =>
            {
                transaction.HasKey(x => x.Id);
            });

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

    }
}
