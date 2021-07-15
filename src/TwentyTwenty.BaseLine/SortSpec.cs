using System;
using System.Linq.Expressions;

namespace TwentyTwenty.BaseLine
{
    public enum ListSortDirection
    {
        Ascending,
        Descending,
    }
    
    public class SortSpec
    {
        public SortSpec(string member, ListSortDirection sortDirection)
        {
            Member = member;
            SortDirection = sortDirection;
        }
        public string Member { get; set; }
        public ListSortDirection SortDirection { get; set; }

        public static SortSpec Create<T>(Expression<Func<T, object>> expression, 
            ListSortDirection direction = ListSortDirection.Descending)
        {
            var name = expression.GetName();
            return new SortSpec(name, direction);
        }
    }
}