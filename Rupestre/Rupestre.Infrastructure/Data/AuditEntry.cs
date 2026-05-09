using System.ComponentModel.DataAnnotations.Schema;

namespace Rupestre.Infrastructure.Data;

[Table("AuditLogs")]
public class AuditEntry
{
    public int      Id              { get; set; }
    public string   UserId          { get; set; } = "";
    public string   Action          { get; set; } = ""; // Create, Update, Delete
    public string   TableName       { get; set; } = "";
    public DateTime DateTime        { get; set; }
    public string   OldValues       { get; set; } = ""; // JSON
    public string   NewValues       { get; set; } = ""; // JSON
    public string   AffectedColumns { get; set; } = ""; // JSON
}
