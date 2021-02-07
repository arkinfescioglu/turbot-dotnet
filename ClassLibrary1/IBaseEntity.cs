using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Data
{
    public interface IBaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTime AtCtreated { get; set; }
        public DateTime AtUpdated { get; set; }
    }

    public class BaseEntity<TId> : IBaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTime AtCtreated { get; set; } = DateTime.Now;
        public DateTime AtUpdated { get; set; }
    }
}
