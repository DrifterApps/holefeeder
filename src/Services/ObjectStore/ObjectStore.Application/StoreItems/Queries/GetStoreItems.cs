﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Http;

using OneOf;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

public static class GetStoreItems
{
    public record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<OneOf<ValidationErrorsRequestResult, ListRequestResult>>, IRequestQuery, IValidateable
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string offsetKey = "offset";
            const string limitKey = "limit";
            const string sortKey = "sort";
            const string filterKey = "filter";

            var hasOffset = int.TryParse(context.Request.Query[offsetKey], out var offset);
            var hasLimit = int.TryParse(context.Request.Query[limitKey], out var limit);
            var sort = context.Request.Query[sortKey].ToArray();
            var filter = context.Request.Query[filterKey].ToArray();

            var result = new Request(hasOffset ? offset : QueryParams.DEFAULT_OFFSET,
                hasLimit ? limit : QueryParams.DEFAULT_LIMIT, sort, filter);

            return ValueTask.FromResult<Request?>(result);
        }
    }

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        public Validator()
        {
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0).WithMessage("offset_invalid");
            RuleFor(x => x.Limit).GreaterThan(0).WithMessage("limit_invalid");
        }

        public OneOf<ValidationErrorsRequestResult, ListRequestResult> CreateResponse(ValidationResult result) =>
            new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public Handler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, ListRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var (total, items) =
                await _itemsQueriesRepository.FindAsync((Guid)_cache["UserId"], QueryParams.Create(request),
                    cancellationToken);

            return new ListRequestResult(total, items);
        }
    }
}
