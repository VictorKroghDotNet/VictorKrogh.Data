using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using VictorKrogh.Data.EntityFrameworkCore.Models;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers;

public abstract class EFCoreDbProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : EFCoreProviderBase<TDbContext>(isolationLevel, dbContext), IEFCoreProvider<TDbContext> where TDbContext : DbContext
{
    public TDbContext DbContext => Context;
}

public abstract class EFCoreProviderBase<TDbContext>(IsolationLevel isolationLevel, TDbContext dbContext) : ProviderBase(isolationLevel), IEFCoreProvider where TDbContext : DbContext
{
    private IDbTransaction? transaction;
    private IDbConnection? connection;
    private bool isCommitted = false;

    protected TDbContext Context => dbContext;

    private IDbConnection Connection => connection ??= CreateConnection();

    private IDbTransaction Transaction
    {
        get
        {
            if (transaction is not null)
            {
                return transaction;
            }

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            return transaction = Context.Database.BeginTransaction(IsolationLevel).GetDbTransaction();
        }
    }

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
        var ct = await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).ToArrayAsync().ConfigureAwait(false);
    }

    public async ValueTask<TModel> QueryFirstAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstAsync().ConfigureAwait(false);
    }

    public async ValueTask<TModel?> QueryFirstOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async ValueTask<TModel> QuerySingleAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleAsync().ConfigureAwait(false);
    }

    public async ValueTask<TModel?> QuerySingleOrDefaultAsync<TModel>(string sql, params object[] parameters) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.SqlQueryRaw<TModel>(sql, parameters).SingleOrDefaultAsync().ConfigureAwait(false);
    }

    public async ValueTask<TModel?> GetAsync<TModel, TKey>(TKey key)
        where TModel : EFCoreModel
        where TKey : notnull
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.FindAsync<TModel>(key).ConfigureAwait(false);
    }

    [Obsolete("Not implemented for EntityFrameworkCore. Use QueryAsync instead.", true)]
    public ValueTask<IEnumerable<TModel?>> GetAllAsync<TModel>() where TModel : EFCoreModel
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> InsertAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return (await Context.AddAsync<TModel>(model).ConfigureAwait(false)).State == EntityState.Added;
    }

    public async ValueTask<bool> UpdateAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return Context.Update<TModel>(model).State == EntityState.Modified;
    }

    public async ValueTask<bool> DeleteAsync<TModel>(TModel model) where TModel : EFCoreModel
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return Context.Remove<TModel>(model).State == EntityState.Deleted;
    }

    public async ValueTask<int> ExecuteAsync(string sql, params object[] parameters)
    {
        await Context.Database.UseTransactionAsync((DbTransaction)Transaction).ConfigureAwait(false);

        return await Context.Database.ExecuteSqlRawAsync(sql, parameters).ConfigureAwait(false);
    }

    public override void Commit()
    {
        if (transaction == null)
        {
            return;
        }

        transaction.Commit();
        isCommitted = true;
    }

    public override void Rollback()
    {
        if (transaction == null)
        {
            return;
        }

        transaction.Rollback();
    }

    protected override void DisposeManagedState()
    {
        if (transaction != null)
        {
            if (!isCommitted)
            {
                Rollback();
            }

            transaction.Dispose();
        }

        if (connection != null)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            connection.Dispose();
        }
    }
}
