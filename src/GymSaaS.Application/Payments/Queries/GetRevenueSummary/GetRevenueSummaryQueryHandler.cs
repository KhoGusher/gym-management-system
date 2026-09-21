using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Payments.Queries.GetRevenueSummary;

public class GetRevenueSummaryQueryHandler : IRequestHandler<GetRevenueSummaryQuery, RevenueSummaryDto>
{
    private readonly IAppDbContext _dbContext;

    public GetRevenueSummaryQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<RevenueSummaryDto> Handle(GetRevenueSummaryQuery request, CancellationToken cancellationToken)
    {
        var paymentsInRange = _dbContext.Payments
            .Where(p => p.PaidAtUtc >= request.FromUtc && p.PaidAtUtc <= request.ToUtc);

        // Step 1: SQL-translatable aggregation into an anonymous type — EF Core handles this fine.
        var grouped = await paymentsInRange
            .GroupBy(p => p.PaidAtUtc.Date)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(p => p.Amount - p.DiscountAmount),
                Count = g.Count()
            })
            .OrderBy(g => g.Date)
            .ToListAsync(cancellationToken);

        // Step 2: map to the real DTO record client-side, now that data is in memory.
        var dailyBreakdown = grouped
            .Select(g => new DailyRevenueDto(DateOnly.FromDateTime(g.Date), g.Total, g.Count))
            .ToList();

        var totalRevenue = dailyBreakdown.Sum(d => d.TotalRevenue);
        var totalDiscounts = await paymentsInRange.SumAsync(p => p.DiscountAmount, cancellationToken);
        var totalPayments = dailyBreakdown.Sum(d => d.PaymentCount);

        return new RevenueSummaryDto(totalRevenue, totalDiscounts, totalPayments, dailyBreakdown);
    }

}