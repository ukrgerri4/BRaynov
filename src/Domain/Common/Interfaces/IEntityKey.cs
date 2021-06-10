using System;

namespace Domain.Common.Interfaces
{
    public interface IEntityKey<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
