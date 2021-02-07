using Microsoft.EntityFrameworkCore.Infrastructure;
using OliWorkshop.Turbo.Data;
using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public class SqlKitImplementation : IDatabaseSetup
    {
        public void DbSetup(DbSetupParameters parameters, DbContextOptionsBuilder options)
        {
            switch (parameters.TypeProvider)
            {
                case "Mysql":
                    options.UseMySql(parameters.Connection, (b) => {
#pragma warning disable EF1001 // Internal EF Core API usage.
                        BasicSetup(b, parameters);
#pragma warning restore EF1001 // Internal EF Core API usage.
                        b.ServerVersion(Version.Parse(parameters.DbVersion),
                             (bool) parameters.Extra ? ServerType.MariaDb : ServerType.MySql);
                    });
                    break;

                case "Sqlite":
                    break;

                case "Prgssql":
                    break;

                case "Memory":
                    options.UseInMemoryDatabase(parameters.Connection);
                    break;

                // if the provider is unknow
                default:
                    break; throw new NotSupportedException("The provider is not supported for " + parameters.TypeProvider);
            }
        }

        private void BasicSetup<T, K>(RelationalDbContextOptionsBuilder<T, K> builder,
            DbSetupParameters parameters)
        where T : RelationalDbContextOptionsBuilder<T, K>
        where K : RelationalOptionsExtension, new()
        {
            if (parameters.Timeout != null)
            {
                builder.CommandTimeout(parameters.Timeout);
            }

            if (parameters.BacthMax != null)
            {
                builder.MaxBatchSize((int)parameters.BacthMax);
            }
        }
    }
}
