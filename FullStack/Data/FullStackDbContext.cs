using FullStack.Models;
using Microsoft.EntityFrameworkCore;

public class FullStackDbContext : DbContext
{
    public FullStackDbContext(DbContextOptions<FullStackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Citizen> Citizens { get; set; }
    public DbSet<AddressRecord> Addresses { get; set; }
    public DbSet<IdentityDocument> Documents { get; set; }
    public DbSet<LoginDetails> Logins { get; set; }
    public DbSet<CitizenRecord> CitizenRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CitizenRecord>()
            .Property(r => r.RecordID)
            .UseIdentityColumn(40000, 1); // Start from 40000

        modelBuilder.Entity<CitizenRecord>()
            .HasOne(r => r.Citizen)
            .WithMany()
            .HasForeignKey(r => r.CitizenID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CitizenRecord>()
            .HasOne(r => r.Address)
            .WithMany()
            .HasForeignKey(r => r.AddressID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CitizenRecord>()
            .HasOne(r => r.Document)
            .WithMany()
            .HasForeignKey(r => r.DocumentID)
            .OnDelete(DeleteBehavior.Restrict);
            
       
        modelBuilder.Entity<AddressRecord>()
            .HasOne(a => a.Citizen)
            .WithMany()
            .HasForeignKey(a => a.CitizenID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IdentityDocument>()
            .HasOne(d => d.Citizen)
            .WithMany()
            .HasForeignKey(d => d.CitizenID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}