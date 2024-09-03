using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Models.Response;

/// <summary>
/// Represents the response for a payment retrieval.
/// </summary>
public record GetPaymentResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the payment.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the status of the payment.
    /// </summary>
    public PaymentStatus Status { get; init; }

    /// <summary>
    /// Gets or sets the last four digits of the card number.
    /// </summary>
    public int CardNumberLastFour { get; init; }

    /// <summary>
    /// Gets or sets the expiry month of the card.
    /// </summary>
    public required int ExpiryMonth { get; init; }

    /// <summary>
    /// Gets or sets the expiry year of the card.
    /// </summary>
    public int ExpiryYear { get; init; }

    /// <summary>
    /// Gets or sets the currency of the payment.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets or sets the amount of the payment.
    /// </summary>
    public int Amount { get; init; }
}
