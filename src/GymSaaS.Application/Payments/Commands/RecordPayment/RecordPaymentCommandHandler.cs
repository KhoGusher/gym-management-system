using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Payments.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, PaymentReceiptDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentTenantService _currentTenantService;

    public RecordPaymentCommandHandler(IAppDbContext dbContext, ICurrentTenantService currentTenantService)
    {
        _dbContext = dbContext;
        _currentTenantService = currentTenantService;
    }

    public async Task<PaymentReceiptDto> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken)
            ?? throw new KeyNotFoundException("Member not found.");

        if (request.MemberSubscriptionId is Guid subId)
        {
            var subscriptionExists = await _dbContext.MemberSubscriptions.AnyAsync(ms => ms.Id == subId, cancellationToken);
            if (!subscriptionExists)
                throw new KeyNotFoundException("Subscription not found.");
        }

        var receiptNumber = await GenerateReceiptNumberAsync(cancellationToken);

        var payment = new Payment
        {
            MemberId = member.Id,
            MemberSubscriptionId = request.MemberSubscriptionId,
            ReceiptNumber = receiptNumber,
            Amount = request.Amount,
            DiscountAmount = request.DiscountAmount,
            Method = request.Method,
            Notes = request.Notes
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PaymentReceiptDto(payment.Id, payment.ReceiptNumber, payment.Amount, payment.DiscountAmount, payment.NetAmount, payment.PaidAtUtc);
    }

    private async Task<string> GenerateReceiptNumberAsync(CancellationToken cancellationToken)
    {
        // Count existing payments FOR THIS TENANT (query filter already scopes this) to derive the next sequence number.
        // Simple and correct for a single-writer-at-a-time gym front desk; see note below on concurrency.
        var count = await _dbContext.Payments.CountAsync(cancellationToken);
        var prefix = _currentTenantService.TenantId?.ToString("N")[..4].ToUpperInvariant() ?? "GEN";
        return $"{prefix}-{(count + 1):D6}";
    }
}