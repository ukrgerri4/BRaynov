using Domain.Common.Models;
using System.Collections.Generic;

namespace Domain.Common.Interfaces
{
    public interface IHasDomainEvent
    {
        List<DomainEvent> DomainEvents { get; set; }
    }
}
