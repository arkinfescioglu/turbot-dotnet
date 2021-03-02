using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get();

        Task<List<TEntity>> FindAll();

        Task<List<TEntity>> Find(int page, int length = 25);

        Task<List<TEntity>> Find(QueryOptions query,int page, int length = 25);

        Task<List<TEntity>> Find(Func<IQueryable<TEntity>, IQueryable<TEntity>> Predicate, int page = 0, int length = 25);

        Task<TEntity> FindOne<TId>(TId id);
        Task<TEntity> FindBy(params object[] fieldsValue);

        Task<IRecord<TEntity>> FindAsRecord<TId>(TId id);

        Task<TEntity> FindOneAndUpdate<TId>(TId id, Action<TEntity> updater);

        bool Validate(TEntity entity);

        Task<int> StoreAnsyc(TEntity entity);

        IRecord<TEntity> CreateCrecord(TEntity entity);

        Task<int> UpdateAsync(TEntity data);

        Task<int> UpdateAsync(TEntity data, TEntity entity);

        Task<int> DeleteAsync<TId>(TId id);

        Task<int> DeleteAsync<TId>(IEnumerable<TId> id);

        Task<int> DeleteAsync(TEntity data);

        Task<int> DeleteAsync(IEnumerable<TEntity> data);

    }
}
