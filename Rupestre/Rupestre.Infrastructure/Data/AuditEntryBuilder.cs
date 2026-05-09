using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Rupestre.Infrastructure.Data;

/// <summary>
/// Acumula dados de uma entrada do ChangeTracker antes do SaveChanges para montar o AuditEntry depois.
/// Necessário porque IDs auto-gerados só ficam disponíveis após a persistência.
/// </summary>
internal sealed class AuditEntryBuilder
{
    private readonly string _action;

    public string?                     UserId             { get; set; }
    public string                      TableName          { get; set; } = "";
    public Dictionary<string, object?> OldValues          { get; } = new();
    public Dictionary<string, object?> NewValues          { get; } = new();
    public List<string>                AffectedColumns    { get; } = new();
    public List<PropertyEntry>         TemporaryProperties { get; } = new();

    public AuditEntryBuilder(EntityEntry entry)
    {
        _action = entry.State switch
        {
            EntityState.Added   => "Create",
            EntityState.Deleted => "Delete",
            _                   => "Update"
        };
    }

    /// <summary>
    /// Chamado após base.SaveChangesAsync para preencher IDs gerados pelo banco.
    /// </summary>
    public AuditEntry Build()
    {
        foreach (var prop in TemporaryProperties)
            NewValues[prop.Metadata.Name] = prop.CurrentValue;

        return new AuditEntry
        {
            UserId          = UserId ?? "Sistema",
            Action          = _action,
            TableName       = TableName,
            DateTime        = DateTime.UtcNow,
            OldValues       = OldValues.Count > 0
                                  ? JsonSerializer.Serialize(OldValues)
                                  : "",
            NewValues       = NewValues.Count > 0
                                  ? JsonSerializer.Serialize(NewValues)
                                  : "",
            AffectedColumns = AffectedColumns.Count > 0
                                  ? JsonSerializer.Serialize(AffectedColumns)
                                  : ""
        };
    }
}
