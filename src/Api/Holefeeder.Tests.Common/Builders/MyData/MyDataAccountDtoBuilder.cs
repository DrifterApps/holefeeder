using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataAccountDtoBuilder : FakerBuilder<MyDataAccountDto>
{
    protected override Faker<MyDataAccountDto> FakerRules { get; } = new Faker<MyDataAccountDto>()
            .RuleFor(f => f.Id, faker => faker.Random.Guid())
            .RuleFor(f => f.Name, faker => faker.Lorem.Word())
            .RuleFor(f => f.OpenBalance, faker => faker.Finance.Amount())
            .RuleFor(f => f.OpenDate, faker => faker.Date.RecentDateOnly())
            .RuleFor(f => f.Description, faker => faker.Lorem.Sentence())
            .RuleFor(f => f.Favorite, faker => faker.Random.Bool())
            .RuleFor(f => f.Inactive, faker => faker.Random.Bool())
            .RuleFor(f => f.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        ;

    public static MyDataAccountDtoBuilder GivenMyAccountData() => new();
}
