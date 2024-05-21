namespace Auctions.Helpers;

public static class PaginationHelper
{
    public static PaginatedList<T> GetPaginatedList<T>(List<T> source, int? pageNumber, int pageSize = 3)
    {
        return  PaginatedList<T>.Create(source, pageNumber ?? 1, pageSize);
    }
}
