namespace Rupestre.Web.Models.DataTables;

public class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesSearch? Search { get; set; }
    public IList<DataTablesOrder>? Order { get; set; }
    public IList<DataTablesColumn>? Columns { get; set; }

    public string SearchValue => Search?.Value ?? string.Empty;
    public string OrderDir => Order?.Count > 0 ? (Order[0].Dir ?? "asc") : "asc";
    public string GetOrderColumn(string defaultColumn = "nome")
    {
        if (Order?.Count > 0 && Columns?.Count > Order[0].Column)
            return Columns[Order[0].Column].Data ?? defaultColumn;
        return defaultColumn;
    }
}

public class DataTablesSearch
{
    public string? Value { get; set; }
}

public class DataTablesOrder
{
    public int Column { get; set; }
    public string? Dir { get; set; }
}

public class DataTablesColumn
{
    public string? Data { get; set; }
    public string? Name { get; set; }
}
