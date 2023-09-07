namespace Domain.Base;

public class AuditableEntity
{
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}