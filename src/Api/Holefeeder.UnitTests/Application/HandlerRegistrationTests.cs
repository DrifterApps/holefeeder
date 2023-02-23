﻿using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace Holefeeder.UnitTests.Application;

public class HandlerRegistrationTests
{
    [Fact]
    public void GivenAllRequests_WhenDefined_ShouldHaveMatchingHandler()
    {
        List<Type> requestTypes = typeof(Api.Api).Assembly.GetTypes()
            .Where(IsRequest)
            .ToList();

        List<Type> handlerTypes = typeof(Api.Api).Assembly.GetTypes()
            .Where(IsIRequestHandler)
            .ToList();

        foreach (Type requestType in requestTypes)
        {
            ShouldContainHandlerForRequest(handlerTypes, requestType);
        }
    }

    private static void ShouldContainHandlerForRequest(IEnumerable<Type> handlerTypes, Type requestType) =>
        handlerTypes.Should().ContainSingle(handlerType => IsHandlerForRequest(handlerType, requestType),
            $"Handler for type {requestType} expected");

    private static bool IsRequest(Type type) => typeof(IBaseRequest).IsAssignableFrom(type);

    private static bool IsIRequestHandler(Type type) =>
        type.GetInterfaces().Any(interfaceType =>
            interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

    private static bool IsHandlerForRequest(Type handlerType, Type requestType) => handlerType.GetInterfaces()
        .Any(i => i.GenericTypeArguments.Any(ta => ta == requestType));
}
