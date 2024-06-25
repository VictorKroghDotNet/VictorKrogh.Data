using VictorKrogh.Data.EntityFrameworkCore.Models;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public interface IEFCoreProvider : IProvider
{
    ValueTask<IEnumerable<TModel?>> QueryAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel;
    ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel;
    ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel;
    ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel;
    ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel;
    ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : EFCoreModel;
    ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : EFCoreModel;
    ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : EFCoreModel;
    ValueTask<int> ExecuteAsync(string sql, params object[] parameters);
}
