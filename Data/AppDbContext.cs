using ElsaTraining.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElsaTraining.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApprovalRequest>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);
    }
}