using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Models.Response;

/// <summary>
/// Represents the response after a payment is processed.
/// </summary>
public record PostPaymentResponse
{
    /// <summary>
    /// Gets the unique identifier of the payment.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the status of the payment.
    /// </summary>
    public PaymentStatus Status { get; init; }

    /// <summary>
    /// Gets the last four digits of the card number used for the payment.
    /// </summary>
    public int CardNumberLastFour { get; init; }

    /// <summary>
    /// Gets the expiry month of the card used for the payment.
    /// </summary>
    public required int ExpiryMonth { get; init; }

    /// <summary>
    /// Gets the expiry year of the card used for the payment.
    /// </summary>
    public int ExpiryYear { get; init; }

    /// <summary>
    /// Gets the currency in which the payment was made.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the amount of the payment.
    /// </summary>
    public int Amount { get; init; }
}
