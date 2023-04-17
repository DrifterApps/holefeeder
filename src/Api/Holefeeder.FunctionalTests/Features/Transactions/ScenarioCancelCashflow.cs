using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CancelCashflowRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioCancelCashflow : BaseScenario
{
    public ScenarioCancelCashflow(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (applicationDriver == null)
        {
            throw new ArgumentNullException(nameof(applicationDriver));
        }
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = GivenAnInvalidCancelCashflowRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCancelsACashflow(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenCancellingACashflow()
    {
        Account account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Cashflow cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Request request = GivenACancelCashflowRequest().WithId(cashflow.Id).Build();

        GivenUserIsAuthorized();

        await WhenUserCancelsACashflow(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        Cashflow? result = await DatabaseDriver.FindByIdAsync<Cashflow>(cashflow.Id);

        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(cashflow, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserCancelsACashflow(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.CancelCashflow, json);
    }
}
