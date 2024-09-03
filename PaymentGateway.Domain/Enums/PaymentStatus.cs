namespace PaymentGateway.Domain.Enums;

/// <summary>
/// Represents the status of the payment.
/// </summary>
public enum PaymentStatus
{
    Authorized,
    Declined,
    Rejected,
    ErrorOccuredOrPending
}