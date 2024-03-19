using System.Data;

namespace VictorKrogh.Data.Providers;

public abstract class ProviderBase(IsolationLevel isolationLevel) : Disposable, IProvider
{
    public IsolationLevel IsolationLevel => isolationLevel;

    public virtual void Commit()
    {
    }

    public virtual void Rollback()
    {
    }
}
