using Microsoft.Extensions.Logging;

using PaymentGateway.Application.BankSimulator;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Repository;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Application.Services
{
    /// <summary>
    /// Service for handling payment operations.
    /// </summary>
    public class PaymentServices(IPaymentRepository paymentRepository,
        IBankAdapter bankAdapter,
        ILogger<PaymentServices> logger) : IPaymentServices
    {
        private const int InvalidCvv = 333;

        /// <inheritdoc/>
        public async Task<PostPaymentResponse> AddPaymentAsync(PostPaymentRequest payment)
        {
            var paymentId = Guid.NewGuid();

            if (!ValidatePaymentInformation(payment))
            {
                logger.LogWarning("Payment with {paymentId} with invalid CVV was rejected.", paymentId);
                return CreatePostPaymentResponse(payment, paymentId, PaymentStatus.Rejected);
            }

            logger.LogInformation("Processing payment {paymentId} with amount {amount} and currency {currency}.", paymentId, payment.Amount, payment.Currency);
            BankResponse bankResponse = await bankAdapter.ProcessPaymentAsync(BuildBankRequest(payment));

            var status = GetPaymentStatus(bankResponse);
            var postPaymentResponse = CreatePostPaymentResponse(payment, paymentId, status);

            await paymentRepository.AddAsync(postPaymentResponse);
            logger.LogInformation("Payment with id {paymentId} and status {status} was added to the database.", paymentId, postPaymentResponse.Status);

            return postPaymentResponse;
        }

        /// <inheritdoc/>
        public async Task<GetPaymentResponse> GetPaymentAsync(Guid id)
        {
            return await paymentRepository.GetAsync(id);
        }

        private static BankRequest BuildBankRequest(PostPaymentRequest payment)
        {
            return new BankRequest
            {
                CardNumber = payment.CardNumber.ToString(),
                ExpiryDate = $"{payment.ExpiryMonth:D2}/{payment.ExpiryYear}",
                Amount = payment.Amount,
                Currency = payment.Currency,
                Cvv = payment.Cvv,
            };
        }

        private static bool ValidatePaymentInformation(PostPaymentRequest payment)
        {
            return payment.Cvv != InvalidCvv;
        }

        private static PostPaymentResponse CreatePostPaymentResponse(PostPaymentRequest payment, Guid paymentId, PaymentStatus status)
        {
            return new PostPaymentResponse
            {
                Id = paymentId,
                Status = status,
                CardNumberLastFour = payment.CardNumber.ToString().GetLastFourDigits(),
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                Amount = payment.Amount,
                Currency = payment.Currency,
            };
        }

        private static PaymentStatus GetPaymentStatus(BankResponse bankResponse)
        {
            var status = bankResponse.ProcessingStatus switch
            {
                BankProcessingStatus.Success => PaymentStatus.Authorized,
                BankProcessingStatus.Failure => PaymentStatus.Declined,
                BankProcessingStatus.InternalError => PaymentStatus.ErrorOccuredOrPending,
                _ => PaymentStatus.ErrorOccuredOrPending,
            };
            return status;
        }
    }
}
