namespace Common.Base;

public abstract class PagedResultBase
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
    public int TotalPages => (int)Math.Ceiling(RowCount / (decimal)PageSize);
    public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
}

public class PagedResult<T> : PagedResultBase where T : class
{
    //Constructor
    public PagedResult() => Results = new List<T>();
    public IList<T> Results { get; set; }
}