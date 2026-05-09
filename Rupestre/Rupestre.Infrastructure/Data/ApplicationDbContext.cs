using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Rupestre.Infrastructure.Identity;

namespace Rupestre.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<AuditEntry> AuditLogs => Set<AuditEntry>();

    // ── Override principal ────────────────────────────────────────────────────

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var builders = CollectAuditBuilders();

        var result = await base.SaveChangesAsync(cancellationToken);

        if (builders.Count > 0)
        {
            foreach (var b in builders)
                AuditLogs.Add(b.Build());

            // Salva os registros de auditoria direto na base sem reentrar no override.
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    // ── Coleta de mudanças ────────────────────────────────────────────────────

    private List<AuditEntryBuilder> CollectAuditBuilders()
    {
        ChangeTracker.DetectChanges();

        var userId = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var builders = new List<AuditEntryBuilder>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditEntry)                    continue;
            if (entry.State is EntityState.Detached
                            or EntityState.Unchanged)          continue;

            var builder = new AuditEntryBuilder(entry)
            {
                UserId    = userId,
                TableName = entry.Metadata.GetTableName()
                            ?? entry.Metadata.ClrType.Name
            };

            foreach (var prop in entry.Properties)
            {
                // Propriedades temporárias (IDs auto-gerados) só terão valor após o save.
                if (prop.IsTemporary)
                {
                    builder.TemporaryProperties.Add(prop);
                    continue;
                }

                var name = prop.Metadata.Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        builder.NewValues[name] = prop.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        builder.OldValues[name] = prop.OriginalValue;
                        break;

                    case EntityState.Modified when prop.IsModified:
                        builder.AffectedColumns.Add(name);
                        builder.OldValues[name] = prop.OriginalValue;
                        builder.NewValues[name]  = prop.CurrentValue;
                        break;
                }
            }

            builders.Add(builder);
        }

        return builders;
    }
}
