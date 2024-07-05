using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VictorKrogh.Data.Providers;
using VictorKrogh.Data.Repositories;

namespace VictorKrogh.Data;

public class UnitOfWork(IServiceProvider serviceProvider, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) : Disposable, IUnitOfWork
{
    protected IServiceProvider ServiceProvider => serviceProvider;
    public IsolationLevel IsolationLevel => isolationLevel;
    public bool IsCompleted { get; set; }

    protected IList<IProvider> Providers { get; set; } = [];

    public void Commit()
    {
        foreach (var provider in Providers)
        {
            provider.Commit();
        }

        IsCompleted = true;
    }

    public virtual TProvider CreateProvider<TProvider>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) where TProvider : IProvider
    {
        var providerFactory = serviceProvider.GetService<IProviderFactory<TProvider>>() ?? throw new Exception($"No provider factory found for {typeof(TProvider).Name}");

        var provider = providerFactory.CreateProvider(isolationLevel);

        Providers.Add(provider);

        return provider;
    }

    public TRepository GetRepository<TRepository>() where TRepository : IRepository
    {
        return serviceProvider.GetRequiredService<TRepository>();
    }

    protected override void DisposeManagedState()
    {
        Providers.Clear();
    }
}
