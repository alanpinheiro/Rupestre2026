namespace Rupestre.Domain.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int TotalRecords { get; set; }
    public int FilteredRecords { get; set; }
}
