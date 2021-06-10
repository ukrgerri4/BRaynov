using Domain.Common.Models;

namespace Domain.Entities
{
    public class Language: BaseEntity<int>
    {
        public string Code { get; set; }
    }
}
