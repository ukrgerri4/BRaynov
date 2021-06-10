using Domain.Common.Models;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Author : BaseEntity<int>
    {
        public string Name { get; set; }
        public ICollection<Profile> TranslatedProfiles { get; set; } = new List<Profile>();
        public ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
    }
}
