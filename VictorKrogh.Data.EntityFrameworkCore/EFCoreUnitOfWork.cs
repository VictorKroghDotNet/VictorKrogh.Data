using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VictorKrogh.Data.EntityFrameworkCore.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore;

public interface IEFCoreUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    TProvider CreateEFProvider<TProvider>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) where TProvider : IEFCoreProvider;
}

public class EFCoreUnitOfWork<TDbContext>(IServiceProvider serviceProvider, TDbContext dbContext, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) : UnitOfWork(serviceProvider, isolationLevel), IEFCoreUnitOfWork<TDbContext>
    where TDbContext : DbContext
{
    public TProvider CreateEFProvider<TProvider>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) where TProvider : IEFCoreProvider
    {
        var providerFactory = ServiceProvider.GetRequiredService<IEFCoreProviderFactory<TProvider, TDbContext>>();

        var provider = providerFactory.CreateProvider(dbContext, isolationLevel);

        Providers.Add(provider);

        return provider;
    }
}
