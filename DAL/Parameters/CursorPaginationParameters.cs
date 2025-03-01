namespace DAL.Parameters;

public class CursorPaginationParameters
{
    public int? LastId { get; set; } = null;
    public int Limit { get; set; } = 10;
}