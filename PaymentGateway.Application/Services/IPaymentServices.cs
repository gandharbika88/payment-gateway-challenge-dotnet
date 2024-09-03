using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Application.Services
{
    /// <summary>
    /// Represents an interface for handling payment operations.
    /// </summary>
    public interface IPaymentServices
    {
        /// <summary>
        /// Adds a new payment asynchronously.
        /// </summary>
        /// <param name="payment">The payment request details.</param>
        /// <returns>The response details of the added payment.</returns>
        Task<PostPaymentResponse> AddPaymentAsync(PostPaymentRequest payment);

        /// <summary>
        /// Retrieves a payment asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The response details of the retrieved payment.</returns>
        Task<GetPaymentResponse> GetPaymentAsync(Guid id);
    }
}
