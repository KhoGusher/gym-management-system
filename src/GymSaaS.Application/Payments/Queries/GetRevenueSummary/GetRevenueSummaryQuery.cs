using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Payments.Queries.GetRevenueSummary;

public record DailyRevenueDto(DateOnly Date, decimal TotalRevenue, int PaymentCount);

public record RevenueSummaryDto(decimal TotalRevenue, decimal TotalDiscounts, int TotalPayments, IReadOnlyList<DailyRevenueDto> DailyBreakdown);

public record GetRevenueSummaryQuery(DateTime FromUtc, DateTime ToUtc) : IRequest<RevenueSummaryDto>;