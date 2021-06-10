using Domain.Common.Models;

namespace Domain.Entities.Implementations
{
    public abstract class Implementation : BaseEntity<int>
    {
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
    }
}
