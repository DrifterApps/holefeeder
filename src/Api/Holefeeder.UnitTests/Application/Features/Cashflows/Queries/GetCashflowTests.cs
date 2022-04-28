using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Cashflows;
using Holefeeder.Application.Features.Cashflows.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Tests.Common.Factories;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Cashflows.Queries.GetCashflow;

namespace Holefeeder.UnitTests.Application.Features.Cashflows.Queries;

public class GetCashflowTests
{
    private readonly AutoFaker<Request> _faker = new();

    private readonly CashflowInfoViewModelFactory _viewModelFactory = new();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly ICashflowQueriesRepository _repositoryMock = Substitute.For<ICashflowQueriesRepository>();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenHandler_WhenIdNotFound_ThenThrowException()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<CashflowNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();
        var transaction = _viewModelFactory.Generate();

        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(transaction);

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(transaction);
    }
}
