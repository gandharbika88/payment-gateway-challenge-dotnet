namespace PaymentGateway.Domain.Enums
{
    /// <summary>
    /// Represents the status of a bank processing operation.
    /// </summary>
    public enum BankProcessingStatus
    {
        Success,
        Failure,
        InternalError
    }
}
