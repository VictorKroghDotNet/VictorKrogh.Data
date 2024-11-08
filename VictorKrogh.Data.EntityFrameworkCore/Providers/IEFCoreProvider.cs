using Microsoft.EntityFrameworkCore;
using VictorKrogh.Data.Models;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public interface IEFCoreProvider<TDbContext> : IEFCoreProvider where TDbContext : DbContext
{
    TDbContext DbContext { get; }
}

public interface IEFCoreProvider : IProvider
{
    DbSet<TModel> GetDbSet<TModel>() where TModel : class, IModel;

    ValueTask<IEnumerable<TModel?>> QueryAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel;
    ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel;
    ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel;
    ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel;
    ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel;

    ValueTask<TModel?> GetAsync<TModel, TKey>(TKey key) where TModel : class, IModel where TKey : notnull;
    ValueTask<IEnumerable<TModel?>> GetAllAsync<TModel>() where TModel : class, IModel;

    ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : class, IModel;
    ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : class, IModel;
    ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : class, IModel;
    ValueTask<int> ExecuteAsync(string sql, params object[] parameters);
}
