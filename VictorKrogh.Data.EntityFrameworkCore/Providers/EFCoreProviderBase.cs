using Microsoft.EntityFrameworkCore;
using System.Data;
using VictorKrogh.Data.EntityFrameworkCore.Models;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public abstract class EFCoreDbProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : EFCoreProviderBase<TDbContext>(isolationLevel, dbContext), IEFCoreProvider<TDbContext> where TDbContext : DbContext
{
    public TDbContext DbContext => Context;
}

public abstract class EFCoreProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : ProviderBase(isolationLevel), IEFCoreProvider where TDbContext : DbContext
{
    protected TDbContext Context => dbContext;

    public DbSet<TModel> GetDbSet<TModel>() where TModel : EFCoreModel
    {
        return Context.Set<TModel>();
    }

    public async ValueTask<IEnumerable<TModel?>> QueryAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).ToArrayAsync();
    }

    public async ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstAsync();
    }

    public async ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstOrDefaultAsync();
    }

    public async ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleAsync();
    }

    public async ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleOrDefaultAsync();
    }

    public ValueTask<TModel?> GetAsync<TModel, TKey>(TKey key)
        where TModel : EFCoreModel
        where TKey : notnull
    {
        return Context.FindAsync<TModel>(key);
    }

    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public ValueTask<IEnumerable<TModel?>> GetAllAsync<TModel>() where TModel : EFCoreModel
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return (await Context.AddAsync<TModel>(model)).State == EntityState.Added;
    }

    public ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return ValueTask.FromResult(Context.Update<TModel>(model).State == EntityState.Modified);
    }

    public ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return ValueTask.FromResult(Context.Remove<TModel>(model).State == EntityState.Deleted);
    }

    public async ValueTask<int> ExecuteAsync(string sql, params object[] parameters)
    {
        return await Context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public override void Commit()
    {
        Context.Database.CommitTransaction();
    }

    public override void Rollback()
    {
        Context.Database.RollbackTransaction();
    }
}
