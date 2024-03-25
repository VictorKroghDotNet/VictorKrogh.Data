using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VictorKrogh.Data.Providers;
using VictorKrogh.Data.Repositories;

namespace VictorKrogh.Data;

public sealed class UnitOfWork(IServiceProvider serviceProvider, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) : Disposable, IUnitOfWork
{
    public IsolationLevel IsolationLevel => isolationLevel;
    public bool IsCompleted { get; set; }

    private IList<IProvider> Providers { get; set; } = [];

    public void Commit()
    {
        foreach (var provider in Providers)
        {
            provider.Commit();
        }

        IsCompleted = true;
    }

    public TProvider CreateProvider<TProvider>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) where TProvider : IProvider
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
