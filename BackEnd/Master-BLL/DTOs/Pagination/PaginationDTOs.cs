using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Pagination
{
    public record PaginationDTOs
         (
         int pageSize,
         int pageIndex,
         bool IsPagination
         );
}
