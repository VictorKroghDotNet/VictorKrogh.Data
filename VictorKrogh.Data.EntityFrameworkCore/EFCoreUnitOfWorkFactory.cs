using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace VictorKrogh.Data.EntityFrameworkCore;

public interface IEFCoreUnitOfWorkFactory<TDbContext> : IUnitOfWorkFactory where TDbContext : DbContext
{
    IEFCoreUnitOfWork<TDbContext> CreateEFCoreUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}

public sealed class EFCoreUnitOfWorkFactory<TDbContext>(IServiceProvider serviceProvider) : IEFCoreUnitOfWorkFactory<TDbContext> where TDbContext : DbContext
{
    public IEFCoreUnitOfWork<TDbContext> CreateEFCoreUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var dbContext = serviceProvider.GetRequiredService<TDbContext>();

        return new EFCoreUnitOfWork<TDbContext>(serviceProvider, dbContext, isolationLevel);
    }

    public IUnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return CreateEFCoreUnitOfWork(isolationLevel);
    }
}