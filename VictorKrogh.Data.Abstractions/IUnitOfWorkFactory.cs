using System.Data;

namespace VictorKrogh.Data;

public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}

