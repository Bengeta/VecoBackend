namespace VecoBackend.Models;

public class PaginatedListModel<T>
{
    public List<T>? data { get; set; }
    public int currentPage { get; set; }
    public int countPage { get; set; }
    public Boolean isNext { get; set; }
}