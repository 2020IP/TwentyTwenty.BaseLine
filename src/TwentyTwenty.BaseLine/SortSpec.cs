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
    }
}