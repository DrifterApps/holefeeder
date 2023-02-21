using System.Collections.Generic;

using Holefeeder.Domain.Enumerations;

namespace Holefeeder.UnitTests.Domain.Enumerations;

public class DateIntervalTypeTests
{
    public static IEnumerable<object[]> NextDateTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateTime(2014, 1, 9)
            };
            yield return new object[]
            {
                new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateTime(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateTime(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateTime(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 2,
                new DateTime(2015, 4, 16)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 5,
                new DateTime(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 1,
                new DateTime(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 1, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 1, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 2, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 2, 28)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2016, 2, 15), DateIntervalType.Monthly, 1,
                new DateTime(2016, 2, 29)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 3, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 3, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 4, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 4, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 5, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 5, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 6, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 6, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 7, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 7, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 8, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 8, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 9, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 9, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 10, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 10, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 11, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 11, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 12, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 12, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 2,
                new DateTime(2015, 5, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 1,
                new DateTime(2016, 1, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 2, 28), new DateTime(2016, 2, 1), DateIntervalType.Yearly, 1,
                new DateTime(2016, 2, 28)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 7,
                new DateTime(2021, 1, 9)
            };
        }
    }

    public static IEnumerable<object[]> PreviousDateTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateTime(2014, 1, 9)
            };
            yield return new object[]
            {
                new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateTime(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateTime(2015, 4, 2)
            };
            yield return new object[]
            {
                new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateTime(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 2,
                new DateTime(2015, 4, 2)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 5,
                new DateTime(2015, 3, 5)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 1,
                new DateTime(2015, 3, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 1, 15), DateIntervalType.Monthly, 1,
                new DateTime(2014, 12, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 2, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 1, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 3, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 2, 28)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2016, 3, 15), DateIntervalType.Monthly, 1,
                new DateTime(2016, 2, 29)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 4, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 3, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 5, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 4, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 6, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 5, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 7, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 6, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 8, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 7, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 9, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 8, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 10, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 9, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 11, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 10, 31)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 31), new DateTime(2015, 12, 15), DateIntervalType.Monthly, 1,
                new DateTime(2015, 11, 30)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 2,
                new DateTime(2015, 3, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 1,
                new DateTime(2015, 1, 9)
            };
            yield return new object[]
            {
                new DateTime(2014, 2, 28), new DateTime(2017, 2, 1), DateIntervalType.Yearly, 1,
                new DateTime(2016, 2, 28)
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2022, 4, 6), DateIntervalType.Yearly, 7,
                new DateTime(2021, 1, 9)
            };
        }
    }

    public static IEnumerable<object[]> IntervalTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Weekly, 2,
                (From: new DateTime(2015, 4, 2), To: new DateTime(2015, 4, 15))
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Weekly, 5,
                (From: new DateTime(2015, 3, 5), To: new DateTime(2015, 4, 8))
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Monthly, 1,
                (From: new DateTime(2015, 3, 9), To: new DateTime(2015, 4, 8))
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 1), new DateTime(2016, 2, 15), DateIntervalType.Monthly, 2,
                (From: new DateTime(2016, 1, 1), To: new DateTime(2016, 2, 29))
            };
            yield return new object[]
            {
                new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Yearly, 1,
                (From: new DateTime(2015, 1, 9), To: new DateTime(2016, 1, 8))
            };
        }
    }

    public static IEnumerable<object[]> DatesInRangeTestCases
    {
        get
        {
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 3, 1), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 3, 1), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 1, 1), Array.Empty<DateTime>()
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateTime(2014, 2, 2), new DateTime(2014, 3, 1),
                new DateTime(2014, 4, 1), Array.Empty<DateTime>()
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 3, 1),
                new[]
                {
                    new DateTime(2014, 2, 2), new DateTime(2014, 2, 9), new DateTime(2014, 2, 16),
                    new DateTime(2014, 2, 23)
                }
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 3, 1),
                new[]
                {
                    new DateTime(2014, 2, 2), new DateTime(2014, 2, 9), new DateTime(2014, 2, 16),
                    new DateTime(2014, 2, 23)
                }
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 2, 1), Array.Empty<DateTime>()
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 4, 1), new[] {new DateTime(2014, 2, 2), new DateTime(2014, 3, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 4, 1), new[] {new DateTime(2014, 2, 2), new DateTime(2014, 3, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2014, 2, 1), Array.Empty<DateTime>()
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 1, 1),
                new DateTime(2016, 4, 1),
                new[] {new DateTime(2014, 2, 2), new DateTime(2015, 2, 2), new DateTime(2016, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateTime(2014, 2, 2), new DateTime(2014, 2, 2),
                new DateTime(2016, 4, 1),
                new[] {new DateTime(2014, 2, 2), new DateTime(2015, 2, 2), new DateTime(2016, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateTime(2014, 2, 2), new DateTime(2013, 1, 1),
                new DateTime(2014, 2, 2), new[] {new DateTime(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateTime(2014, 2, 2), new DateTime(2013, 1, 1),
                new DateTime(2014, 2, 1), Array.Empty<DateTime>()
            };
        }
    }

    [Theory]
    [MemberData(nameof(NextDateTestCases))]
    public void NextDate_ValidDateTime_NewDateTime(DateTime originalDate, DateTime effectiveDate,
        DateIntervalType intervalType, int frequency, DateTime expected)
    {
        if (intervalType is null)
        {
            throw new ArgumentNullException(nameof(intervalType));
        }

        intervalType.NextDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(PreviousDateTestCases))]
    public void PreviousDate_ValidDateTime_NewDateTime(DateTime originalDate, DateTime effectiveDate,
        DateIntervalType intervalType, int frequency, DateTime expected)
    {
        if (intervalType is null)
        {
            throw new ArgumentNullException(nameof(intervalType));
        }

        intervalType.PreviousDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IntervalTestCases))]
    public void TestInterval(DateTime originalDate, DateTime effectiveDate, DateIntervalType intervalType,
        int frequency, (DateTime From, DateTime To) expected)
    {
        if (intervalType is null)
        {
            throw new ArgumentNullException(nameof(intervalType));
        }

        intervalType.Interval(originalDate, effectiveDate, frequency).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(DatesInRangeTestCases))]
    public void GivenDatesInRange_WhenValidDateTime_ThenListOfDatesInRange(DateIntervalType intervalType,
        int frequency, DateTime effective, DateTime from, DateTime to, DateTime[] expected)
    {
        if (intervalType is null)
        {
            throw new ArgumentNullException(nameof(intervalType));
        }

        intervalType.DatesInRange(effective, from, to, frequency).Should().Equal(expected);
    }
}
