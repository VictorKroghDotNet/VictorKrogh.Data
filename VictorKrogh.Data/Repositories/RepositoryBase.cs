using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.Repositories;

public abstract class RepositoryBase<TProvider>(TProvider provider) : IRepository
    where TProvider : IProvider
{
    protected TProvider Provider => provider;
}
