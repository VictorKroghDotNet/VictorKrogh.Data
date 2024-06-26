using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using VictorKrogh.Data.Providers;

namespace VictorKrogh.Data.EntityFrameworkCore.Providers
{
    public sealed class EFCoreProviderFactory<TProvider, TDbContext>(TDbContext context) : IProviderFactory<TProvider> where TProvider : IEFCoreProvider where TDbContext : DbContext
    {
        public TProvider CreateProvider(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var type = typeof(TProvider);

            var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, [typeof(IsolationLevel), typeof(TDbContext)], null);
            if (ctor == null)
            {
                throw new NotImplementedException("Constructor not implemented.");
            }

            var instance = ctor.Invoke([isolationLevel, context]);

            return (TProvider)instance;
        }
    }
}
