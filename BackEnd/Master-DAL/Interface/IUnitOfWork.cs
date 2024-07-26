using Microsoft.EntityFrameworkCore.Storage;


namespace Master_DAL.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        IDbContextTransaction BeginTransaction();
        Task<int> SaveChangesAsync();
    }
}
