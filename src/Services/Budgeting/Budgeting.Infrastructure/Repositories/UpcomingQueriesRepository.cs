﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Cashflows;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class UpcomingQueriesRepository : IUpcomingQueriesRepository
{
    private readonly IHolefeederContext _context;

    public UpcomingQueriesRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate,
        DateTime endDate, CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var cashflows = await connection
            .QueryAsync<CashflowEntity, AccountEntity, CategoryEntity, CashflowEntity>(@"
SELECT 
    c.*, MAX(t.date) AS last_paid_date, MAX(t.cashflow_date) AS last_cashflow_date,
    a.*, 
    ca.*
FROM cashflows c
INNER JOIN accounts a on a.id = c.account_id
INNER JOIN categories ca on ca.id = c.category_id
LEFT OUTER JOIN transactions t on t.cashflow_id = c.id
WHERE c.user_id = @UserId AND c.inactive = 0
GROUP BY c.id;",
                (entity, accountEntity, categoryEntity) =>
                    entity with { Account = accountEntity, Category = categoryEntity }, new { UserId = userId },
                splitOn: "id,id")
            .ConfigureAwait(false);

        var results = cashflows
            .SelectMany(x =>
            {
                var dates = new List<DateTime>();

                dates.AddRange(x.IntervalType
                    .DatesInRange(x.EffectiveDate, startDate, endDate, x.Frequency)
                    .Where(futureDate =>
                        IsUnpaid(x.EffectiveDate, futureDate, x.LastPaidDate, x.LastCashflowDate)));

                var date = x.IntervalType.PreviousDate(x.EffectiveDate, startDate,
                    x.Frequency);
                while (IsUnpaid(x.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate) &&
                       date > x.EffectiveDate)
                {
                    dates.Add(date);
                    date = x.IntervalType.PreviousDate(x.EffectiveDate, date,
                        x.Frequency);
                }

                return dates.Select(d =>
                    new UpcomingViewModel
                    {
                        Id = x.Id,
                        Date = d,
                        Amount = x.Amount,
                        Description = x.Description,
                        Tags = x.Tags?.ToImmutableArray() ?? ImmutableArray<string>.Empty,
                        Category = new CategoryInfoViewModel(x.Category.Id, x.Category.Name, x.Category.Type,
                            x.Category.Color),
                        Account = new AccountInfoViewModel(x.Account.Id, x.Account.Name)
                    });
            }).Where(x => x.Date <= endDate)
            .OrderBy(x => x.Date);

        return results;
    }

    private static bool IsUnpaid(DateTime effectiveDate, DateTime nextDate, DateTime? lastPaidDate,
        DateTime? lastCashflowDate) =>
        !lastPaidDate.HasValue
            ? (nextDate >= effectiveDate)
            : (nextDate > lastPaidDate && nextDate > lastCashflowDate);
}
