using FC.Consolidado.Domain.Entities;
using FC.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace FC.Consolidado.Infra.Data;

public sealed class ConsolidadoDbContext : DbContext, IUnitOfWork
{
    public ConsolidadoDbContext(DbContextOptions<ConsolidadoDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Transacao> Transacoes { get; set; }

    public async Task<bool> Commit()
    {
        return await SaveChangesAsync() > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Cascade;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsolidadoDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}