﻿using Microsoft.EntityFrameworkCore;
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

    protected virtual IDbConnection CreateConnection()
    {
        return Context.Database.GetDbConnection();
    }

    public DbSet<TModel> GetDbSet<TModel>() where TModel : EFCoreModel
    {
        return Context.Set<TModel>();
    }

    public async ValueTask<IEnumerable<TModel?>> QueryAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).ToArrayAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstOrDefaultAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleOrDefaultAsync().ConfigureAwait(true);
    }

    public async ValueTask<TModel?> GetAsync<TModel, TKey>(TKey key)
        where TModel : EFCoreModel
        where TKey : notnull
    {
        return await Context.FindAsync<TModel>(key).ConfigureAwait(true);
    }

    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public ValueTask<IEnumerable<TModel?>> GetAllAsync<TModel>() where TModel : EFCoreModel
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return (await Context.AddAsync<TModel>(model).ConfigureAwait(true)).State == EntityState.Added;
    }

    public async ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return Context.Update<TModel>(model).State == EntityState.Modified;
    }

    public async ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        return Context.Remove<TModel>(model).State == EntityState.Deleted;
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
