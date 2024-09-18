using X.PagedList;

namespace Sales.Application.Parameters.Extension;

public static class IPagedListExtension
{
    public static object GenerateMetadataHeader<T>(this IPagedList<T> source)
    {
        return new
        {
            source.Count,
            source.PageSize,
            source.PageCount,
            source.TotalItemCount,
            source.HasNextPage,
            source.HasPreviousPage
        };
    }
}