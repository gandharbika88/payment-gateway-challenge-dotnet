using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Application.Repository;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Infrastructure.Persistence
{
    /// <summary>
    /// Repository for managing payment data using an in-memory cache.
    /// </summary>
    public class PaymentsRepository(IMemoryCache memoryCache) : IPaymentRepository
    {
        /// <inheritdoc/>
        public async Task<Guid> AddAsync(PostPaymentResponse payment)
        {
            memoryCache.Set(payment.Id, payment, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
            return await Task.FromResult(payment.Id);
        }

        /// <inheritdoc/>
        public async Task<GetPaymentResponse> GetAsync(Guid id)
        {
            if (!memoryCache.TryGetValue(id, out PostPaymentResponse? payment) || payment == null)
            {
                throw new KeyNotFoundException($"Payment with id {id} not found.");
            }

            GetPaymentResponse paymentResponse = new()
            {
                Id = payment.Id,
                Status = payment.Status,
                CardNumberLastFour = payment.CardNumberLastFour,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                Amount = payment.Amount,
                Currency = payment.Currency,
            };
            return await Task.FromResult(paymentResponse);
        }

        /// <inheritdoc/>
        public async Task<Guid> UpdateAsync(Guid paymentId, PaymentStatus status)
        {
            if (!memoryCache.TryGetValue(paymentId, out PostPaymentResponse? postPaymentResponse) || postPaymentResponse == null)
            {
                throw new KeyNotFoundException($"Payment with id {paymentId} not found.");
            }

            postPaymentResponse = postPaymentResponse with { Status = status };
            memoryCache.Set(paymentId, postPaymentResponse);
            return await Task.FromResult(paymentId);
        }
    }
}
