using System;

namespace Domain.Common.Interfaces
{
    public interface IAuditable
    {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }

        int CreatedBy { get; set; }
        int ModifiedBy { get; set; }
    }
}
