using Domain.Entities.Dtos;
using Domain.Interfaces.Repositories;
using Infra.Data.Context;

namespace Infra.Data.Repositories;

public class TestRepository(IUnitOfWork unitOfWork) : BaseRepository<TestConsumerDto>(unitOfWork), ITestRepository;