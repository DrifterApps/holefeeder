﻿using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using OneOf;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class FavoriteAccountCommandHandlerTests
    {
        [Fact]
        public async Task GivenFavoriteAccountCommand_WhenRequestIsValid_ThenReturnCommandOk()
        {
            // arrange
            var command = new FavoriteAccount.Request(Guid.NewGuid(), false);
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(
                    new Account(Guid.NewGuid(), AccountType.Checking, "Account name", DateTime.Today, Guid.NewGuid())
                    {
                        Favorite = false,
                        OpenBalance = Decimal.One,
                        Description = "Description",
                        Inactive = false,
                        Cashflows = Array.Empty<Guid>()
                    });
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<FavoriteAccount.Handler>>();
            var handler = new FavoriteAccount.Handler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult> expected =
                Unit.Value;
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GivenFavoriteAccountCommand_WhenAccountDoesNotExists_ThenReturnCommandNotFound()
        {
            // arrange
            var command = new FavoriteAccount.Request(Guid.NewGuid(), false);
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<FavoriteAccount.Handler>>();
            var handler = new FavoriteAccount.Handler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult> expected =
                new NotFoundRequestResult();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
