using VictorKrogh.Data.EntityFrameworkCore.Models;
using VictorKrogh.Data.EntityFrameworkCore.Providers;
using VictorKrogh.Data.Repositories;

namespace VictorKrogh.Data.EntityFrameworkCore.Repositories;

public abstract class EFCoreReadOnlyRepositoryBase<TModel>(IEFCoreProvider efCoreProvider) : EFCoreRepositoryBase(efCoreProvider), IReadOnlyRepository<TModel> where TModel : EFCoreModel
{
    protected async ValueTask<IEnumerable<TModel?>> QueryAsync(string sql, params object[] parameters)
    {
        return await Provider.QueryAsync<TModel>(sql, parameters);
    }

    protected async ValueTask<TModel> QueryFirstAsync(string sql, params object[] parameters)
    {
        return await Provider.QueryFirstAsync<TModel>(sql, parameters);
    }

    protected async ValueTask<TModel?> QueryFirstOrDefaultAsync(string sql, params object[] parameters)
    {
        return await Provider.QueryFirstOrDefaultAsync<TModel>(sql, parameters);
    }

    protected async ValueTask<TModel> QuerySingleAsync(string sql, params object[] parameters)
    {
        return await Provider.QuerySingleAsync<TModel>(sql, parameters);
    }

    protected async ValueTask<TModel?> QuerySingleOrDefaultAsync(string sql, params object[] parameters)
    {
        return await Provider.QuerySingleOrDefaultAsync<TModel>(sql, parameters);
    }

    /// <summary>
    /// Not implemented for EntityFrameworkCore. Use QueryAsync instead.
    /// </summary>
    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public abstract ValueTask<IEnumerable<TModel?>> GetAsync();

    /// <summary>
    /// Not implemented for EntityFrameworkCore. Use QueryFirstOrDefault instead.
    /// </summary>
    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryFirstOrDefault instead.", true)]
    public async ValueTask<TModel?> GetFirstOrDefaultAsync()
    {
        var result = await GetAsync();
        if (result == null)
        {
            return default;
        }
        return result.FirstOrDefault();
    }
}

public abstract class EFCoreReadOnlyRepositoryBase<TModel, TKey>(IEFCoreProvider efCoreProvider) : EFCoreReadOnlyRepositoryBase<TModel>(efCoreProvider), IReadOnlyRepository<TModel, TKey>
    where TModel : EFCoreModel
    where TKey : notnull
{
    public async ValueTask<TModel?> GetAsync(TKey key)
    {
        return await Provider.GetAsync<TModel, TKey>(key);
    }

    /// <summary>
    /// Not implemented for EntityFrameworkCore. Use QueryAsync instead.
    /// </summary>
    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public override async ValueTask<IEnumerable<TModel?>> GetAsync()
    {
        return await Provider.GetAllAsync<TModel>();
    }
}
