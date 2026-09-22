using System;

namespace GiftOfTheGivers.Models
{
    public class AuditLog
    {
        public int AuditLogID { get; set; }
        public string EntityName { get; set; } = null!;
        public int EntityID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Details { get; set; }
    }
}