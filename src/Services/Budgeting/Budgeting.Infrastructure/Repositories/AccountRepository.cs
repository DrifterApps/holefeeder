using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IHolefeederContext _context;
    private readonly IMapper _mapper;

    public IUnitOfWork UnitOfWork => _context;

    public AccountRepository(IHolefeederContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Account?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var cashflows = new List<Guid>();

        var account = (await connection
                .QueryAsync<AccountEntity, Guid?, AccountEntity>(@"
SELECT 
    a.*, c.id
FROM accounts a
LEFT OUTER JOIN cashflows c on c.account_id = a.id
WHERE a.id = @Id AND a.user_id = @UserId;
",
                    (account, cashflowId) =>
                    {
                        if (cashflowId is not null)
                        {
                            cashflows.Add(cashflowId.Value);
                        }

                        return account;
                    },
                    new { Id = id, UserId = userId },
                    splitOn: "id")
                .ConfigureAwait(false))
            .Distinct()
            .SingleOrDefault();

        return account is null ? null : _mapper.Map<Account>(account) with { Cashflows = cashflows.ToImmutableList()};
    }

    public async Task<Account?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<AccountEntity>(new {Name = name, UserId = userId})
            .ConfigureAwait(false);

        return _mapper.Map<Account>(schema.FirstOrDefault());
    }

    public async Task SaveAsync(Account account, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = account.Id;
        var userId = account.UserId;

        var entity = await transaction.FindByIdAsync<AccountEntity>(new { Id = id, UserId = userId });

        if (entity is null)
        {
            await transaction.InsertAsync(_mapper.Map<AccountEntity>(account))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(_mapper.Map(account, entity)).ConfigureAwait(false);
        }
    }
}
