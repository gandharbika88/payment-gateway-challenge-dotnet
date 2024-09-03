using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models.Response;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Infrastructure.Tests.Persistence
{
    public class PaymentsRepositoryTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly PaymentsRepository _repository;

        public PaymentsRepositoryTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _repository = new PaymentsRepository(_memoryCache);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPaymentToCache()
        {
            var payment = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 1234,
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "USD"
            };

            var result = await _repository.AddAsync(payment);

            Assert.True(_memoryCache.TryGetValue(result, out PostPaymentResponse? cachedPayment));
            Assert.NotNull(cachedPayment);
            Assert.Equal(payment.Id, cachedPayment.Id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPaymentFromCache()
        {
            var payment = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 1234,
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "USD"
            };

            _memoryCache.Set(payment.Id, payment);

            var result = await _repository.GetAsync(payment.Id);

            Assert.Equal(payment.Id, result.Id);
            Assert.Equal(payment.Status, result.Status);
        }

        [Fact]
        public async Task GetAsync_ShouldThrowKeyNotFoundException_WhenPaymentNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetAsync(nonExistentId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePaymentStatusInCache()
        {
            var payment = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 1234,
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "USD"
            };

            _memoryCache.Set(payment.Id, payment);

            var newStatus = PaymentStatus.Declined;
            var result = await _repository.UpdateAsync(payment.Id, newStatus);

            Assert.True(_memoryCache.TryGetValue(result, out PostPaymentResponse? updatedPayment));
            Assert.NotNull(updatedPayment);
            Assert.Equal(newStatus, updatedPayment.Status);
        }
    }
}
