using System.Text.Json.Serialization;

using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.BankSimulator
{
    /// <summary>
    /// Represents the response from the bank simulator.
    /// </summary>
    public record BankResponse
    {
        /// <summary>
        /// Gets the status of the payment.
        /// </summary>
        [JsonPropertyName("authorized")]
        public bool Authorized { get; init; }

        /// <summary>
        /// Gets the authorization code.
        /// </summary>
        [JsonPropertyName("authorization_code")]
        public string? AuthorizationCode { get; init; }

        /// <summary>
        /// Gets or sets the processing status of the payment.
        /// </summary>
        public BankProcessingStatus ProcessingStatus { get; set; }
    }
}
