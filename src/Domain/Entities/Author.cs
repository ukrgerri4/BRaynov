using Domain.Common.Interfaces;
using System;
using System.Linq.Expressions;

namespace Domain.Entities
{
    public class Author : IAuditable, IVersioned
    {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }
}
