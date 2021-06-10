using Z.EntityFramework.Plus;

namespace Infrastructure.Database.Entities
{
    public class AuditEntryExtended : AuditEntry
    {
        public string RequestId { get; set; }
    }
}
