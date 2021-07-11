using System;

namespace Medius.Model
{
        public abstract class BaseEntity<T>
        {
            public virtual T Id { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime LastModify { get; set; } = DateTime.Now;
            public bool IsActive { get; set; } = true;
        }
}
