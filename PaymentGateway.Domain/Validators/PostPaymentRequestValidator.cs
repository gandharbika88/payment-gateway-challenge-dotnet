using FluentValidation;

using PaymentGateway.Domain.Models.Requests;

namespace PaymentGateway.Domain.Validators
{
    /// <summary>
    /// Validator for PostPaymentRequest.
    /// </summary>
    public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostPaymentRequestValidator"/> class.
        /// </summary>
        public PostPaymentRequestValidator()
        {
            RuleFor(x => x.CardNumber.ToString()).NotEmpty().Must(IsCardNumberValid).WithMessage("Card number must be between 14 and 19 digits");
            RuleFor(x => x.ExpiryMonth).NotEmpty().InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12");
            RuleFor(x => x.ExpiryYear).NotEmpty().InclusiveBetween(2024, 2099).WithMessage("Expiry year must be between 2024 and 2099");
            RuleFor(x => new { x.ExpiryMonth, x.ExpiryYear }).Must(x => IsExpiryInFuture(x.ExpiryMonth, x.ExpiryYear)).WithMessage("Expiry date must be in the future");
            RuleFor(x => x.Currency).NotEmpty().Length(3).Must(IsCurrencyValid).WithMessage("Currency must be GBP, USD or EUR");
            RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Cvv.ToString()).NotEmpty();
            RuleFor(x => x.Cvv).Must(IsCvvValid).WithMessage("CVV must be 3 or 4 digits");
        }

        private static bool IsExpiryInFuture(int expiryMonth, int expiryYear)
        {
            if (expiryYear < 1 || expiryMonth < 1)
            {
                return false;
            }
            DateTime expiryDate = new(expiryYear, expiryMonth, 1);
            return expiryDate.AddMonths(1).AddDays(-1).Date >= DateTime.Now.Date;
        }

        private static bool IsCurrencyValid(string currency)
        {
            return currency == "GBP" || currency == "USD" || currency == "EUR";
        }

        private static bool IsCvvValid(int cvv)
        {
            int length = cvv.ToString().Length;
            return length == 3 || length == 4;
        }

        private static bool IsCardNumberValid(string cardNumber)
        {
            int length = cardNumber.Length;
            return length >= 14 && length <= 19;
        }
    }
}
