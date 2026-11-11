using System.Data.Common;

namespace Lindekilde.Blazor.CruddyModel.Repositories;

/// <summary>
/// Creates Dapper-backed repositories.
/// </summary>
public class DapperRepositoryFactory : IRepositoryFactory
{
    /// <inheritdoc />
    public IRepository<TEntity> Create<TEntity>(DbConnection dbConnection, string schema, string tableName, string? keyColumn) =>
        new DapperRepository<TEntity>(dbConnection, schema, tableName, keyColumn);
}
