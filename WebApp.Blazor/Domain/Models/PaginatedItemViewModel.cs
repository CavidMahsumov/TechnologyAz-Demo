namespace WebApp.Blazor.Domain.Models
{
    public class PaginatedItemViewModel<TEntity> where TEntity : class
    {
        public PaginatedItemViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
        public PaginatedItemViewModel()
        {

        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long Count { get; set; }
        public IEnumerable<TEntity> Data { get; set; }
    }
}
