using Domain.Entities;
using Tests.Mocks;

namespace Tests.Entities;

public abstract class BaseEntityTest<TMock, T>
    where TMock : BaseMock<T>, new()
    where T : BaseEntity
{
    private readonly TMock _mock = new();
    
    [Fact]
    public void ShouldCreate()
    {
        var entity = _mock.GetEntity();
        
        Assert.NotNull(entity);
        Assert.IsType<T>(entity);
    }
}