namespace TestAssignmentWebAPI.Contracts.TaskDtos;

public class PaginationResultDto<T>
{
    public IEnumerable<T> Items { get; set; }  
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPAge => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}