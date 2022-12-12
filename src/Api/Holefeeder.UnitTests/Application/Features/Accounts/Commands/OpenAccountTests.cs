using System.Linq;
using System.Threading.Tasks;

using Holefeeder.Domain.Features.Accounts;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

public class OpenAccountTests
{
    private readonly Faker<Request> _faker;

    public OpenAccountTests()
    {
        _faker = new AutoFaker<Request>()
            .RuleForType(typeof(AccountType), faker => faker.PickRandom(AccountType.List.ToArray()));
    }

    [Fact]
    public async Task GivenValidator_WhenTypeIsNull_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Type, _ => null!).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GivenValidator_WhenNameIsInvalid_ThenError(string name)
    {
        // arrange
        var request = _faker.RuleFor(x => x.Name, _ => name).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenValidator_WhenOpenDateIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.OpenDate, _ => DateTime.MinValue).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.OpenDate);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
