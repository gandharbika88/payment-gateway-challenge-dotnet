namespace PaymentGateway.Application.BankSimulator
{
    /// <summary>
    /// Represents an interface for a bank adapter.
    /// </summary>
    public interface IBankAdapter
    {
        /// <summary>
        /// Processes the payment asynchronously by sending a request to the bank.
        /// </summary>
        /// <param name="bankRequest">The bank request containing payment details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the bank response.</returns>
        Task<BankResponse> ProcessPaymentAsync(BankRequest postPaymentRequest);
    }
}
