using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Reflection;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public interface IEFCoreProviderFactory<TProvider, TDbContext> : IProviderFactory<TProvider> 
    where TProvider : IEFCoreProvider
    where TDbContext : DbContext
{
    TProvider CreateProvider(TDbContext dbContext, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}

public sealed class EFCoreProviderFactory<TProvider, TDbContext>(IServiceProvider serviceProvider) : IEFCoreProviderFactory<TProvider, TDbContext>
    where TProvider : IEFCoreProvider
    where TDbContext : DbContext
{
    public TProvider CreateProvider(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var dbContext = serviceProvider.GetRequiredService<TDbContext>();

        return CreateProvider(dbContext, isolationLevel);
    }

    public TProvider CreateProvider(TDbContext dbContext, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var type = typeof(TProvider);

        var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, [typeof(IsolationLevel), typeof(TDbContext)], null);
        if (ctor == null)
        {
            throw new NotImplementedException("Constructor not implemented.");
        }

        var instance = ctor.Invoke([isolationLevel, dbContext]);

        return (TProvider)instance;
    }
}
