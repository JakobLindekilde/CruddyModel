#pragma warning disable BL0005 // Component parameters are set directly in the unit test.

using System.Data.Common;
using Lindekilde.Blazor.CruddyModel.Components;
using Lindekilde.Blazor.CruddyModel.Repositories;
using Lindekilde.Blazor.CruddyModel.Tests.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lindekilde.Blazor.CruddyModel.Tests.Components;

public class RepositoryFactoryTests
{
    [Fact]
    public async Task Component_UsesRegisteredFactoryDuringInitialization()
    {
        var factory = new RecordingFactory();
        var db = new FakeDbConnection();
        var services = new ServiceCollection();
        services.AddSingleton<IRepositoryFactory>(factory);
        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, NullLoggerFactory.Instance);
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(CruddyModelBase<Person>.DbConnection)] = db,
            [nameof(CruddyModelBase<Person>.DefaultSchema)] = "sales",
            [nameof(CruddyModelBase<Person>.TableName)] = "People",
            [nameof(CruddyModelBase<Person>.KeyColumn)] = "PersonKey"
        });

        await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<CruddyModelBase<Person>>(parameters));

        var call = Assert.Single(factory.Calls);
        Assert.Equal(typeof(Person), call.EntityType);
        Assert.Same(db, call.Connection);
        Assert.Equal(("sales", "People", "PersonKey"), (call.Schema, call.TableName, call.KeyColumn));
        Assert.Equal("SELECT TOP 10000  * FROM sales.People", Assert.Single(db.Commands).Sql);
    }

    [Fact]
    public void Component_PassesCurrentSettingsToFactoryForEachOperation()
    {
        var factory = new RecordingFactory();
        var firstConnection = new FakeDbConnection();
        var secondConnection = new FakeDbConnection();
        var component = new CruddyModelBase<Person>
        {
            DbConnection = firstConnection,
            RepositoryFactory = factory,
            DefaultSchema = "sales",
            TableName = "People",
            KeyColumn = "PersonKey"
        };

        component.Delete(1);
        component.DbConnection = secondConnection;
        component.DefaultSchema = "archive";
        component.TableName = "OldPeople";
        component.KeyColumn = "OldKey";
        component.Delete(2);

        Assert.Equal(2, factory.Calls.Count);
        Assert.Same(firstConnection, factory.Calls[0].Connection);
        Assert.Equal(("sales", "People", "PersonKey"), (factory.Calls[0].Schema, factory.Calls[0].TableName, factory.Calls[0].KeyColumn));
        Assert.Same(secondConnection, factory.Calls[1].Connection);
        Assert.Equal(("archive", "OldPeople", "OldKey"), (factory.Calls[1].Schema, factory.Calls[1].TableName, factory.Calls[1].KeyColumn));
        Assert.Equal("DELETE FROM sales.People WHERE PersonKey = @keyValue", Assert.Single(firstConnection.Commands).Sql);
        Assert.Equal("DELETE FROM archive.OldPeople WHERE OldKey = @keyValue", Assert.Single(secondConnection.Commands).Sql);
    }

    private sealed class RecordingFactory : IRepositoryFactory
    {
        private readonly DapperRepositoryFactory _inner = new();
        public List<FactoryCall> Calls { get; } = [];

        public IRepository<TEntity> Create<TEntity>(DbConnection dbConnection, string schema, string tableName, string? keyColumn)
        {
            Calls.Add(new(typeof(TEntity), dbConnection, schema, tableName, keyColumn));
            return _inner.Create<TEntity>(dbConnection, schema, tableName, keyColumn);
        }
    }

    private sealed record FactoryCall(Type EntityType, DbConnection Connection, string Schema, string TableName, string? KeyColumn);
}
