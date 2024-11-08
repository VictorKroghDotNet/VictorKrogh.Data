namespace VictorKrogh.Data.Repositories;

public interface IRepository
{
}

public interface IRepository<TModel> : IRepository
    where TModel : class
{
}

public interface IWriteRepository<TModel> : IRepository<TModel>
    where TModel : class
{
    ValueTask<bool> AddAsync(TModel model);
    ValueTask<bool> UpdateAsync(TModel model);
    ValueTask<bool> AddOrUpdateAsync(TModel model);
    ValueTask<bool> DeleteAsync(TModel model);
    ValueTask<bool> DeleteAllAsync();
}

public interface IReadOnlyRepository<TModel> : IRepository<TModel>
    where TModel : class
{
    ValueTask<TModel?> GetFirstOrDefaultAsync();
    ValueTask<IEnumerable<TModel?>> GetAsync();
}

public interface IReadOnlyRepository<TModel, TKey1> : IReadOnlyRepository<TModel>
    where TModel : class
    where TKey1 : notnull
{
    ValueTask<TModel?> GetAsync(TKey1 key);
}

public interface IRepository<TModel, TKey1> : IWriteRepository<TModel>, IReadOnlyRepository<TModel, TKey1>
    where TModel : class
    where TKey1 : notnull
{
}