namespace Infra.Data.Context;
public interface IUnitOfWork
{
    protected Context Context { get; }
    Context GetContext();
    void SaveChanges();
    Task SaveChangesAsync();
}
