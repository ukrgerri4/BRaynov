using Domain.Common.Models;

namespace Domain.Entities
{
    public class Profile : BaseEntity<int>
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
    }
}
