using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Data
{
    public interface IRecord<TEntity> 
    {
        public TEntity Entry { get; }

        public void Emit(string emmit);

        public Task SaveAsync(CancellationToken cancellation = default);

        public Task RemoveAsync(CancellationToken cancellation = default);
    }
}
