using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Data
{
    public class EfRecord<TEntity> : IRecord<TEntity>
        where TEntity : class
    {
        public EfRecord(DbContext dbContext, TEntity entity)
        {
            Entry = entity;
            dbContext.Set<TEntity>().Add(entity);
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TEntity Entry { get; }

        private DbContext DbContext { get; }

        public void Emit(string emmit)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(CancellationToken cancellation = default)
        {
            DbContext.Entry(Entry).State = EntityState.Deleted;
            return SaveAsync(cancellation);
        }

        public Task SaveAsync(CancellationToken cancellation = default)
        {
            return DbContext.SaveChangesAsync(cancellation);
        }
    }
}
