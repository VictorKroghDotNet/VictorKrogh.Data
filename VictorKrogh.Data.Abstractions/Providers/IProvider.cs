using System.Data;

namespace VictorKrogh.Data.Providers;

public interface IProvider
{
    IsolationLevel IsolationLevel { get; }

    void Commit();
    void Rollback();
}