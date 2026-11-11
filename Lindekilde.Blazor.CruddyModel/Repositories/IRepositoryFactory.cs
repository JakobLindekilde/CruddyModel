using System.Data.Common;

namespace Lindekilde.Blazor.CruddyModel.Repositories;

/// <summary>
/// Creates repositories using the connection and table settings supplied by a component.
/// </summary>
public interface IRepositoryFactory
{
    /// <summary>
    /// Creates a repository for an entity type and table.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="dbConnection">The connection used by the repository.</param>
    /// <param name="schema">The table schema.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="keyColumn">The primary key column, if specified.</param>
    /// <returns>A repository for the specified table.</returns>
    IRepository<TEntity> Create<TEntity>(DbConnection dbConnection, string schema, string tableName, string? keyColumn);
}
