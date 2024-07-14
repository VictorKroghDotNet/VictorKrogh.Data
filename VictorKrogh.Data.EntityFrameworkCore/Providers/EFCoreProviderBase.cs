using Microsoft.EntityFrameworkCore;
using System.Data;
using VictorKrogh.Data.Models;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public abstract class EFCoreDbProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : EFCoreProviderBase<TDbContext>(isolationLevel, dbContext), IEFCoreProvider<TDbContext> where TDbContext : DbContext
{
    public TDbContext DbContext => Context;
}

public abstract class EFCoreProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : ProviderBase(isolationLevel), IEFCoreProvider where TDbContext : DbContext
{
    protected TDbContext Context => dbContext;

    public DbSet<TModel> GetDbSet<TModel>() where TModel : class, IModel
    {
        return Context.Set<TModel>();
    }

    public async ValueTask<IEnumerable<TModel?>> QueryAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).ToArrayAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstOrDefaultAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : class, IModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleOrDefaultAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> GetAsync<TModel, TKey>(TKey key)
        where TModel : class, IModel
        where TKey : notnull
    {
        return await Context.FindAsync<TModel>(key).ConfigureAwait(true);
    }

    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public async ValueTask<IEnumerable<TModel?>> GetAllAsync<TModel>() where TModel : class, IModel
    {
        return await GetDbSet<TModel>().ToArrayAsync();
    }

    public async ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : class, IModel
    {
        return (await Context.AddAsync<TModel>(model).ConfigureAwait(true)).State == EntityState.Added;
    }

    public async ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : class, IModel
    {
        return await ValueTask.FromResult(Context.Update<TModel>(model).State == EntityState.Modified);
    }

    public async ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : class, IModel
    {
        return await ValueTask.FromResult(Context.Remove<TModel>(model).State == EntityState.Deleted);
    }

    public async ValueTask<int> ExecuteAsync(string sql, params object[] parameters)
    {
        return await Context.Database.ExecuteSqlRawAsync(sql, parameters).ConfigureAwait(true);
    }

    public override void Commit()
    {
        Context.SaveChanges();
    }

    public override void Rollback()
    {
    }

    protected override void DisposeManagedState()
    {
        Context.Dispose();
    }
}
