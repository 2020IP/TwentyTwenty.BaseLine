using System.Collections.Generic;

namespace TwentyTwenty.BaseLine
{
    public interface IPagedList<T>
    {
        int PageSize { get; set; }
        int CurrentPage { get; set; }
        long TotalItems { get; set; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        IList<T> Items { get; }
    }
}