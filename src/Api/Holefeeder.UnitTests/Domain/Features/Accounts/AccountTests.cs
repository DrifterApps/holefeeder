﻿using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.Builders.Accounts;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;

namespace Holefeeder.UnitTests.Domain.Features.Accounts;

public class AccountTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        AccountBuilder builder = GivenAnActiveAccount().WithId(default);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenConstructor_WhenNameIsEmpty_ThenThrowException(string name)
    {
        // arrange
        AccountBuilder builder = GivenAnActiveAccount().WithName(name);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Name must be from 1 to 100 characters")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenNameIsTooLong_ThenThrowException()
    {
        // arrange
        AccountBuilder builder = GivenAnActiveAccount().WithName(_faker.Random.Words().ClampLength(256));

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Name must be from 1 to 100 characters")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenOpenDateIsMissing_ThenThrowException()
    {
        // arrange
        AccountBuilder builder = GivenAnActiveAccount().WithOpenDate(default);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("OpenDate is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange
        AccountBuilder builder = GivenAnActiveAccount().ForNoUser();

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenCloseAccount_WhenClosing_ThenThrowException()
    {
        // arrange
        Account account = GivenAnInactiveAccount().Build();

        // act
        Action action = () => account.Close();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Account already closed")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenOpenAccountWithCashflows_WhenClosing_ThenThrowException()
    {
        // arrange
        Account account = GivenAnActiveAccount().WithActiveCashflows().Build();

        // act
        Action action = () => account.Close();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Account has active cashflows")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GivenAccount_WhenSetAsFavorite_ThenAccountIsModified(bool favorite)
    {
        // arrange
        Account account = GivenAnActiveAccount().IsFavorite(!favorite).Build();

        // act
        account = account with { Favorite = favorite };

        // assert
        account.Favorite.Should().Be(favorite);
    }
}
