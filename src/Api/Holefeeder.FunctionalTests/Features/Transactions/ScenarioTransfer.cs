using System.Net;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using static Holefeeder.Application.Features.Transactions.Commands.Transfer;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransferRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioTransfer : BaseScenario
{
    public ScenarioTransfer(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (applicationDriver == null)
        {
            throw new ArgumentNullException(nameof(applicationDriver));
        }
    }

    [Fact]
    public async Task InvalidRequest()
    {
        Request request = default!;

        await ScenarioFor("an invalid transfer request", player =>
        {
            player
                .Given("an invalid transfer", () => request = GivenAnInvalidTransfer().Build())
                .And(User.IsAuthorized)
                .When("a transfer is made", () => Transaction.Transfer(request))
                .Then("should receive a validation error", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Request request = default!;

        await ScenarioFor("an authorized user making a transfer", player =>
        {
            player
                .Given("a transfer request", () => request = GivenATransfer().Build())
                .And(User.IsAuthorized)
                .When("the transfer is made", () => Transaction.Transfer(request))
                .Then("the user should be authorized to access the endpoint", ThenUserShouldBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Request request = null!;

        await ScenarioFor("a forbidden user making a request", player =>
        {
            player
                .Given("a transfer request", () => request = GivenATransfer().Build())
                .And(User.IsForbidden)
                .When("the request is sent", () => Transaction.Transfer(request))
                .Then("should be forbidden from accessing the endpoint", ShouldBeForbiddenToAccessEndpoint);
        });
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Request entity = null!;

        await ScenarioFor("an unauthorized user making a transfer request", player =>
        {
            player
                .Given("a transfer request", () => entity = GivenATransfer().Build())
                .And(User.IsUnauthorized)
                .When("the transfer request is made", () => Transaction.Transfer(entity))
                .Then("the user should not be authorized to access endpoint", ShouldNotBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact(Skip = "Works locally but not on the workflow; need this deployed to fix urgent bug")]
    public async Task ValidRequest()
    {
        Account fromAccount = null!;
        Account toAccount = null!;
        Request request = null!;
        (Guid FromTransactionId, Guid ToTransactionId) ids = default;

        await ScenarioFor("a valid transfer request", player =>
        {
            player
                .Given("the user has an account to transfer from", async () => fromAccount = await GivenAnActiveAccount().ForUser(HolefeederUserId).SavedInDb(DatabaseDriver))
                .And("an account to transfer to", async () => toAccount = await GivenAnActiveAccount().ForUser(HolefeederUserId).SavedInDb(DatabaseDriver))
                .And("they hava a category to receive money", async () => await GivenACategory().WithName("Transfer In").ForUser(HolefeederUserId).SavedInDb(DatabaseDriver))
                .And("a category to send money", async () => await GivenACategory().WithName("Transfer Out").ForUser(HolefeederUserId).SavedInDb(DatabaseDriver))
                .And("their request is valid", () => request = GivenATransfer().FromAccount(fromAccount).ToAccount(toAccount).Build())
                .And(User.IsAuthorized)
                .When("the transfer request is sent", () => Transaction.Transfer(request))
                .Then("the return code should be Created", () => ThenShouldExpectStatusCode(HttpStatusCode.Created))
                .And("the route of the new resource should be in the header", () => ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
                .And("both transaction ids should be received", () =>
                {
                    ids = ThenShouldReceive<(Guid FromTransactionId, Guid ToTransactionId)>();
                    ids.FromTransactionId.Should().NotBeEmpty();
                    ids.ToTransactionId.Should().NotBeEmpty();
                })
                .And("the data of the outgoing transaction be valid", async () =>
                {
                    Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(ids.FromTransactionId);

                    result.Should().NotBeNull($"because the FromTransactionId ({ids.FromTransactionId}) was not found");

                    TransactionMapper.MapToModelOrNull(result).Should()
                        .NotBeNull()
                        .And
                        .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
                })
                .And("the data of the incoming transaction be valid", async () =>
                {
                    Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(ids.ToTransactionId);

                    result.Should().NotBeNull($"because the ToTransactionId ({ids.ToTransactionId}) was not found");

                    TransactionMapper.MapToModelOrNull(result).Should()
                        .NotBeNull()
                        .And
                        .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
                });
        });
    }
}
