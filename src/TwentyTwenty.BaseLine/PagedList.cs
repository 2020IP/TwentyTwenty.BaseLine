using System;
using System.Linq;
using System.Collections.Generic;

namespace TwentyTwenty.BaseLine
{
    public class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            Items = new List<T>();
        }

        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageSize == 0)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Page size can not be 0");
            }
            if (pageNumber == 0)
            {
                throw new ArgumentOutOfRangeException("pageNumber", "Page number can not be 0");
            }
            
            var skip = (pageNumber - 1) * pageSize;
            CurrentPage = pageNumber;
            PageSize = pageSize;            
            TotalItems = source.Count();

            Items = new List<T>(source.Skip(skip).Take(pageSize).ToList());
        }
        
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public long TotalItems { get; set; }
        public IList<T> Items { get; set; }

        public int TotalPages
            => (int)Math.Ceiling((decimal)TotalItems / PageSize);        
        public bool HasPreviousPage
            => CurrentPage > 1;
        public bool HasNextPage
            => CurrentPage < TotalPages;        
    }
}