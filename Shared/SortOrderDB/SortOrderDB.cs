namespace Shared.SortOrderDB
{
    public enum SortOrderDB
    {
        Price_Desc,
        Price_Asc,
        DateModified_Desc,
        DateModified_Asc,
    }
    public struct SortOrderDBData
    {
        public SortOrderDBData(SortOrderDB sortOrder)
        {
            var arr = sortOrder.ToString().Split("_");
            Row = arr[0];
            Direction = arr[1];
        }
        public string Row;
        public string Direction;
    }
   

}
