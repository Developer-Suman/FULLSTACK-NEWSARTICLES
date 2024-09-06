using Master_BLL.DTOs.Pagination;
using Master_DAL.Abstraction;
using Microsoft.EntityFrameworkCore;


namespace ExtensionMethods.Pagination
{
    public static class IQueryableExtension
    {
        public static async Task<Result<PagedResult<T>>> ToPagedResultAsync<T>(this IQueryable<T> query, int? pageIndex, int? pageSize, bool IsPagination)
        {
            try
            {
                List<T> items;
                int totalItems = await query.CountAsync();

                if (IsPagination)
                {
                    int validPageIndex = pageIndex ?? 1;
                    int validPageSize = pageSize ?? 10;

                    if (validPageIndex < 1)
                    {
                        validPageIndex = 1;
                    }

                    if (validPageSize < 1)
                    {
                        validPageSize = 10;
                    }

                    items = await query.Skip((validPageIndex - 1) * validPageSize).Take(validPageSize).ToListAsync();

                    var pagedResult = new PagedResult<T>
                    {
                        Items = items,
                        TotalItems = totalItems,
                        PageIndex = validPageIndex,
                        PageSize = validPageSize
                    };
                    return Result<PagedResult<T>>.Success(pagedResult);
                }
                else
                {


                    // Fetch all data without pagination
                    items = await query.ToListAsync();

                    return Result<PagedResult<T>>.Success(new PagedResult<T>
                    {
                        Items = items,
                        TotalItems = totalItems,
                        PageIndex = 1,  // Default to 1
                        PageSize = totalItems  // Set PageSize to the total number of items
                    });
                }




            }
            catch (Exception ex)
            {
                return Result<PagedResult<T>>.Failure("NotFound", "Getting proble while fetching data");
            }
        }

    }
}



