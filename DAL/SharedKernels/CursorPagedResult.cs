namespace DAL.SharedKernels;

public class CursorPagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int? NextLastId { get; set; } = null;
    public bool HasMore { get; set; }
    
    public CursorPagedResult(IEnumerable<T> items, int? nextLastId, bool hasMore)
    {
        Items = items;
        NextLastId = nextLastId;
        HasMore = hasMore;
    }
}