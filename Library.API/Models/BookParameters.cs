namespace Library.API.Models
{
    public class BookParameters
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public string SortBy { get; set; } = "Title";
        public bool SortDescending { get; set; } = false;
    }
}