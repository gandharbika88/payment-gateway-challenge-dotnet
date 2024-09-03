using System.Text.Json.Serialization;

namespace PaymentGateway.Application.BankSimulator
{
    /// <summary>
    /// Represents a request object to the bank.
    /// </summary>
    public record BankRequest
    {
        /// <summary>
        /// Gets the card number.
        /// </summary>
        [JsonPropertyName("card_number")]
        public required string CardNumber { get; init; }

        /// <summary>
        /// Gets the expiry date of the card.
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public required string ExpiryDate { get; init; }

        /// <summary>
        /// Gets the currency of the transaction.
        /// </summary>
        public required string Currency { get; init; }

        /// <summary>
        /// Gets the amount of the transaction.
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// Gets the CVV of the card.
        /// </summary>
        public int Cvv { get; init; }
    }
}
