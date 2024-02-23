using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity>  Repository<TEntity>() where TEntity : class;

        IDbContextTransaction BeginTransaction();
        Task<int> SaveChangesAsync();
    }
}
