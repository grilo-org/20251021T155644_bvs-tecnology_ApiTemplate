namespace Domain.Entities;

public abstract class BaseEntity(Guid? id = null)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public Guid? CreatedBy { get; protected set; } = id;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected void Update() => UpdatedAt = DateTime.UtcNow;
}