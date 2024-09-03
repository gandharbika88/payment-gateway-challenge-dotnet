namespace PaymentGateway.Domain.Models.Requests;

/// <summary>
/// Represents a request to post a payment.
/// </summary>
public record PostPaymentRequest
{
    /// <summary>
    /// Gets the card number.
    /// </summary>
    public required double CardNumber { get; init; }

    /// <summary>
    /// Gets the expiry month of the card.
    /// </summary>
    public required int ExpiryMonth { get; init; }

    /// <summary>
    /// Gets the expiry year of the card.
    /// </summary>
    public required int ExpiryYear { get; init; }

    /// <summary>
    /// Gets the currency of the payment.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the amount of the payment.
    /// </summary>
    public required int Amount { get; init; }

    /// <summary>
    /// Gets the CVV of the card.
    /// </summary>
    public required int Cvv { get; init; }
}
