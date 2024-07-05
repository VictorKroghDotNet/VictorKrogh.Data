using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace VictorKrogh.Data.EntityFrameworkCore;

public sealed class EFCoreUnitOfWorkFactory<TDbContext>(IServiceProvider serviceProvider) : IUnitOfWorkFactory where TDbContext : DbContext
{
    public IUnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var dbContext = serviceProvider.GetRequiredService<TDbContext>();

        return new EFCoreUnitOfWork<TDbContext>(serviceProvider, dbContext, isolationLevel);
    }
}