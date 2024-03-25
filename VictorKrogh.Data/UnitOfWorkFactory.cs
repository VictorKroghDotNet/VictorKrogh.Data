using System.Data;

namespace VictorKrogh.Data;

public sealed class UnitOfWorkFactory(IServiceProvider serviceProvider) : IUnitOfWorkFactory
{
    public IUnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return new UnitOfWork(serviceProvider, isolationLevel);
    }
}