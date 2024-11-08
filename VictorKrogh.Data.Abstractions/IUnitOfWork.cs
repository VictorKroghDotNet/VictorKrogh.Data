using System.Data;
using VictorKrogh.Data.Providers;
using VictorKrogh.Data.Repositories;

namespace VictorKrogh.Data;

public interface IUnitOfWork : IProviderFactory, IDisposable
{
    IsolationLevel IsolationLevel { get; }
    bool IsCompleted { get; }
    void Commit();
    TRepository GetRepository<TRepository>() where TRepository : IRepository;
}

