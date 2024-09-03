using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Application.Repository
{
    /// <summary>
    /// Represents an interface for a payment repository.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Adds a new payment to the cache.
        /// </summary>
        /// <param name="payment">The payment details to add.</param>
        /// <returns>The ID of the added payment.</returns>
        Task<Guid> AddAsync(PostPaymentResponse payment);

        /// <summary>
        /// Retrieves a payment from the cache by its ID.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The payment details.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the payment with the specified ID is not found.</exception>
        Task<Guid> UpdateAsync(Guid paymentId, PaymentStatus status);

        /// <summary>
        /// Updates the status of an existing payment in the cache.
        /// </summary>
        /// <param name="paymentId">The ID of the payment to update.</param>
        /// <param name="status">The new status of the payment.</param>
        /// <returns>The ID of the updated payment.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the payment with the specified ID is not found.</exception>
        Task<GetPaymentResponse> GetAsync(Guid id);
    }
}
