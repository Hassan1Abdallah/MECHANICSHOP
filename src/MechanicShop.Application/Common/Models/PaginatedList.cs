
namespace MechanicShop.Application.Common.Models
{
    public class PaginatedList<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyCollection<T>? Items { get; set; }

    }
}
