namespace OrderManagementAPI.DTOs;
public record PagedResult<T>(
    List<T> Items,
    int page,
    int pageSize,
    int totalCount
)
{
    public int TotalPages=>(int)Math.Ceiling(totalCount/(double)pageSize);
    public bool HasPreviousPage=>page>1;
    public bool HasNextPage=>page<TotalPages;
}