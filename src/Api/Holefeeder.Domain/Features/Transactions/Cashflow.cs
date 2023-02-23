﻿using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Domain.Features.Transactions;

public record Cashflow : Entity, IAggregateRoot
{
    private readonly Guid _accountId;
    private readonly decimal _amount;
    private readonly Guid _categoryId;
    private readonly DateTime _effectiveDate;
    private readonly int _frequency;
    private readonly Guid _id;
    private readonly int _recurrence;
    private readonly Guid _userId;

    public sealed override Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(Id)} is required", nameof(Cashflow));
            }

            _id = value;
        }
    }

    public DateTime EffectiveDate
    {
        get => _effectiveDate;
        init
        {
            if (value.Equals(default))
            {
                throw new TransactionDomainException($"{nameof(EffectiveDate)} is required", nameof(Cashflow));
            }

            _effectiveDate = value;
        }
    }


    public DateIntervalType IntervalType { get; init; } = DateIntervalType.Weekly;

    public int Frequency
    {
        get => _frequency;
        init
        {
            if (value <= 0)
            {
                throw new TransactionDomainException($"{nameof(Frequency)} must be positive", nameof(Cashflow));
            }

            _frequency = value;
        }
    }

    public int Recurrence
    {
        get => _recurrence;
        init
        {
            if (value < 0)
            {
                throw new TransactionDomainException($"{nameof(Recurrence)} cannot be negative", nameof(Cashflow));
            }

            _recurrence = value;
        }
    }

    public decimal Amount
    {
        get => _amount;
        init
        {
            if (value < 0m)
            {
                throw new TransactionDomainException($"{nameof(Amount)} cannot be negative", nameof(Cashflow));
            }

            _amount = value;
        }
    }

    public string Description { get; init; } = string.Empty;

    public Guid AccountId
    {
        get => _accountId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(AccountId)} is required", nameof(Cashflow));
            }

            _accountId = value;
        }
    }

    public Account? Account { get; init; }

    public Guid CategoryId
    {
        get => _categoryId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(CategoryId)} is required", nameof(Cashflow));
            }

            _categoryId = value;
        }
    }

    public Category? Category { get; init; }

    public IReadOnlyCollection<string> Tags { get; private set; } = ImmutableList<string>.Empty;

    public IReadOnlyCollection<Transaction> Transactions { get; init; } = new List<Transaction>();

    public DateTime? LastPaidDate => Transactions.Any() ? Transactions.Max(x => x.Date) : null;

    public DateTime? LastCashflowDate => Transactions.Max(x => x.CashflowDate);

    public bool Inactive { get; init; }

    public Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(UserId)} is required", nameof(Cashflow));
            }

            _userId = value;
        }
    }

    public static Cashflow Create(DateTime effectiveDate, DateIntervalType intervalType, int frequency, int recurrence,
        decimal amount, string description, Guid categoryId, Guid accountId, Guid userId) =>
        new Cashflow
        {
            Id = Guid.NewGuid(),
            EffectiveDate = effectiveDate,
            Amount = amount,
            UserId = userId,
            IntervalType = intervalType,
            Frequency = frequency,
            Recurrence = recurrence,
            Description = description,
            CategoryId = categoryId,
            AccountId = accountId
        };

    public Cashflow Cancel()
    {
        if (Inactive)
        {
            throw new TransactionDomainException($"Cashflow {Id} already inactive", nameof(Cashflow));
        }

        return this with { Inactive = true };
    }

    public Cashflow SetTags(params string[] tags)
    {
        List<string> newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        Tags = newTags.ToImmutableArray();
        return this;
    }

    public IReadOnlyCollection<DateTime> GetUpcoming(DateTime to)
    {
        List<DateTime> dates = new List<DateTime>();

        if (Inactive)
        {
            return dates;
        }

        dates.AddRange(IntervalType
            .DatesInRange(EffectiveDate, EffectiveDate, to, Frequency)
            .Where(futureDate => IsUnpaid(EffectiveDate, futureDate)));

        return dates;
    }

    private bool IsUnpaid(DateTime effectiveDate, DateTime nextDate) =>
        LastPaidDate is null
            ? nextDate >= effectiveDate
            : nextDate > LastPaidDate && nextDate > LastCashflowDate;
}
