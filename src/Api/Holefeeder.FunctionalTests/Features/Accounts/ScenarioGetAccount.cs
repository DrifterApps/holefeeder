using System.Net;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetAccount : HolefeederScenario
{
    public ScenarioGetAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountExistsWithExpenses()
    {
        Account account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel? result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        AssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    account.Id,
                    account.Type,
                    account.Name,
                    account.OpenBalance,
                    account.OpenDate,
                    TransactionCount = 1,
                    Balance = decimal.Subtract(account.OpenBalance, transaction.Amount),
                    Updated = transaction.Date,
                    account.Description,
                    account.Favorite,
                    account.Inactive
                });
        });
    }

    [Fact]
    public async Task WhenAccountExistsWithGains()
    {
        Account account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Gain)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel? result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        AssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    account.Id,
                    account.Type,
                    account.Name,
                    account.OpenBalance,
                    account.OpenDate,
                    TransactionCount = 1,
                    Balance = decimal.Add(account.OpenBalance, transaction.Amount),
                    Updated = transaction.Date,
                    account.Description,
                    account.Favorite,
                    account.Inactive
                });
        });
    }

    private async Task WhenUserGetAccount(Guid id) => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetAccount, new object[] { id.ToString() });
}
