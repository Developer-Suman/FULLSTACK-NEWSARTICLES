using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetConditionalAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task AddRange(List<TEntity> entity);
        void DeleteRange(List<TEntity> entity);
        Task DeleteAllAsync();
    }
}
