using Domain.Entities;

namespace Tests.Mocks;

public abstract class BaseMock<T> where T : BaseEntity
{
    protected Faker _faker = new();
    public T GetEntity() => GetEntity(_faker.Random.Guid());
    protected virtual T GetEntity(Guid id) => It.IsAny<T>();
    public IEnumerable<T> GetEmptyEnumerable() => [];
    public IEnumerable<T> GetEnumerable(int quantity) => Enumerable.Range(0, quantity).Select(_ => GetEntity());
}