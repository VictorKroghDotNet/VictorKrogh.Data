using VictorKrogh.Data.EntityFrameworkCore.Models;
using VictorKrogh.Data.EntityFrameworkCore.Providers;
using VictorKrogh.Data.Repositories;

namespace VictorKrogh.Data.EntityFrameworkCore.Repositories;

public abstract class EFCoreRepositoryBase(IEFCoreProvider provider) : RepositoryBase<IEFCoreProvider>(provider)
{
    protected async ValueTask<int> ExecuteAsync(string sql, params object[] parameters)
    {
        return await Provider.ExecuteAsync(sql, parameters);
    }
}

public abstract class EFCoreRepositoryBase<TModel, TKey>(IEFCoreProvider provider) : EFCoreReadOnlyRepositoryBase<TModel, TKey>(provider), IRepository<TModel, TKey>
    where TModel : EFCoreModel
    where TKey : notnull
{
    public async virtual ValueTask<bool> AddAsync(TModel model)
    {
        return await Provider.InsertAsync(model);
    }

    public async virtual ValueTask<bool> UpdateAsync(TModel model)
    {
        return await Provider.UpdateAsync(model);
    }

    public async virtual ValueTask<bool> AddOrUpdateAsync(TModel model)
    {
        if (model.IsTransient())
        {
            return await AddAsync(model);
        }

        return await UpdateAsync(model);
    }

    public async virtual ValueTask<bool> DeleteAsync(TModel model)
    {
        return await Provider.DeleteAsync(model);
    }

    /// <summary>
    /// Not implemented for EntityFrameworkCore.
    /// </summary>
    [Obsolete("Not implemented for EntityFrameworkCore.", true)]
    public virtual ValueTask<bool> DeleteAllAsync()
    {
        throw new NotImplementedException();
    }
}