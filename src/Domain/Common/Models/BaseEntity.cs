using Domain.Common.Interfaces;
using System;

namespace Domain.Common.Models
{
    public abstract class BaseEntity<TKey> : IEntityKey<TKey>, IAuditable, IVersioned where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public byte[] RowVersion { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }
}
