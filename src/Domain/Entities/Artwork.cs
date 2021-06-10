using Domain.Common.Models;
using Domain.Entities.Implementations;
using Domain.Enums;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Artwork : BaseEntity<int>
    {
        public string Name { get; set; }
        public ArtworkType Type { get; set; }
        public ICollection<Implementation> Implementations { get; set; } = new List<Implementation>();
    }
}
