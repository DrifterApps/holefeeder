using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;

namespace Holefeeder.FunctionalTests.Features.Statistics;

public class ScenarioGetForAllCategories : BaseScenario
{
    private readonly Guid _userId = MockAuthenticationHandler.AuthorizedUserId;

    private readonly Dictionary<string, Category> _categories = new();
    private readonly Dictionary<string, Account> _accounts = new();

    private readonly ITestOutputHelper _testOutputHelper;

    public ScenarioGetForAllCategories(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task WhenGettingStatistics()
    {
        await ScenarioPlayer.Create("A user sends was to know his statistics", _testOutputHelper)
            .Given("the user is authorized", GivenUserIsAuthorized)
            .And("a 'purchase' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("purchase", "credit card", new DateTime(2023, 2, 1), 500.5m))
            .And("a 'food and drink' transaction was made on the 'credit card' account in December 2022", () => CreateTransaction("food and drink", "credit card", new DateTime(2022, 12, 1), 100.1m))
            .And("a 'food and drink' transaction was made on the 'credit card' account in January 2023", () => CreateTransaction("food and drink", "credit card", new DateTime(2023, 1, 1), 200.2m))
            .And("a 'food and drink' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("food and drink", "credit card", new DateTime(2023, 2, 1), 300.3m))
            .And("a second 'food and drink' transaction was made on the 'checking' account in February 2023", () => CreateTransaction("food and drink", "checking", new DateTime(2023, 2, 10), 400.4m))
            .When("user gets their statistics", () => WhenUserTriesToQuery(ApiResource.GetForAllCategories))
            .Then("the total for the year should match the expected", ValidateResponse)
            .PlayAsync();
    }

    private void ValidateResponse()
    {
        var results = HttpClientDriver.DeserializeContent<StatisticsDto[]>();
        results.Should()
            .NotBeNull()
            .And.SatisfyRespectively(
                first => first.Should().BeEquivalentTo(new StatisticsDto(_categories["food and drink"].Id,
                    _categories["food and drink"].Name,
                    new[]
                    {
                        new YearStatisticsDto(2022, 100.1m, new[] {new MonthStatisticsDto(12, 100.1m)}),
                        new YearStatisticsDto(2023, 900.9m,
                            new[] {new MonthStatisticsDto(1, 200.2m), new MonthStatisticsDto(2, 700.7m)})
                    })),
                second => second.Should().BeEquivalentTo(new StatisticsDto(_categories["purchase"].Id,
                    _categories["purchase"].Name,
                    new[] { new YearStatisticsDto(2023, 500.5m, new[] { new MonthStatisticsDto(2, 500.5m) }) }))
            );
    }

    private async Task CreateTransaction(string categoryName, string accountName, DateTime date, decimal amount)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder.GivenACategory().WithName(categoryName).ForUser(_userId).SavedInDb(DatabaseDriver);
            _categories.Add(categoryName, category);
        }

        if (!_accounts.TryGetValue(accountName, out var account))
        {
            account = await AccountBuilder.GivenAnActiveAccount().WithName(accountName).ForUser(_userId).SavedInDb(DatabaseDriver);
            _accounts.Add(accountName, account);
        }

        await TransactionBuilder
            .GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .OnDate(date)
            .OfAmount(amount)
            .SavedInDb(DatabaseDriver);
    }
}
