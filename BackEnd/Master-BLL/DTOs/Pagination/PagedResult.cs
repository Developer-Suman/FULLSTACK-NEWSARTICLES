using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public int? FirstPage => TotalPages > 0 ? 1 : (int?)null;
        public int? LastPage => TotalPages > 0 ? TotalPages : (int?)null;
        public int? PreviousPage => PageIndex > 1 ? PageIndex - 1 : (int?)null;
        public int? NextPage => PageIndex < TotalPages ? PageIndex + 1 : (int?)null;
    }
}
