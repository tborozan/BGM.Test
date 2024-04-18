using BGM.Test.Web.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BGM.Test.Web.DAL;

public class BgmDbContext : DbContext
{
    public BgmDbContext(DbContextOptions<BgmDbContext> options): base(options)
    {
        
    }

    public virtual DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>()
            .HasIndex(x => x.IsPaid);
    }
}