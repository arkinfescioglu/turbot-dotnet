using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Data
{
    public interface IRecord<TEntity> 
    {
        public TEntity Entry { get; }

        public Task SaveAsync();

        public void Load(Func<TEntity, object> loader);

        public void Emit(string emmit);

        public void Detach();

        public Task RemoveAsync();
    }
}
