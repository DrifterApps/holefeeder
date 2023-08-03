using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure.Persistence;
using Holefeeder.Application.Context;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DatabaseDriver<BudgetingContext>
{
    private const string Schema = "budgeting_tests";

    protected override IDatabaseServer DatabaseServer { get; init; }

    protected override RespawnerOptions Options { get; } = new()
    {
        DbAdapter = DbAdapter.MySql,
        SchemasToInclude = new[] { Schema },
        TablesToInclude = new Table[] { "accounts", "cashflows", "categories", "store_items", "transactions" },
        TablesToIgnore = new Table[] { "schema_versions" },
        WithReseed = true
    };

    public BudgetingDatabaseDriver()
    {
        var connectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            DatabaseServer = MariaDatabaseServer.CreateServer(Schema);
        }
        else
        {
            DatabaseServer = LocalDatabaseServer.CreateServer(connectionString!, ConnectionFactory);
        }
    }

    private static MySqlConnection ConnectionFactory(string connectionString) => new(connectionString);

    public override async Task InitializeAsync()
    {
        await DatabaseServer.StartAsync();

        await base.InitializeAsync();
    }

    public override BudgetingContext CreateDbContext()
    {
        var context = new BudgetingContext(GetDbContextOptions());
        return context;
    }

    public override DbContextOptions<BudgetingContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BudgetingContext>();

        optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));

        return optionsBuilder.Options;
    }

    protected override async Task InitializeDatabaseAsync()
    {
        await using var dbContext = new BudgetingContext(GetDbContextOptions());

        await dbContext.Database.EnsureCreatedAsync();
        dbContext.Database.MigrateDb(new BudgetingConnectionStringBuilder { ConnectionString = ConnectionString });
    }
}
