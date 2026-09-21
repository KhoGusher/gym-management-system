using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Payments.Queries.GetMemberPaymentHistory;

public class GetMemberPaymentHistoryQueryHandler : IRequestHandler<GetMemberPaymentHistoryQuery, IReadOnlyList<PaymentHistoryItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetMemberPaymentHistoryQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<PaymentHistoryItemDto>> Handle(GetMemberPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Payments
            .Where(p => p.MemberId == request.MemberId)
            .OrderByDescending(p => p.PaidAtUtc)
            .Select(p => new PaymentHistoryItemDto(p.Id, p.ReceiptNumber, p.Amount, p.DiscountAmount, p.NetAmount, (int)p.Method, p.Notes, p.PaidAtUtc))
            .ToListAsync(cancellationToken);
    }
} 