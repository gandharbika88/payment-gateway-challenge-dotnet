namespace PaymentGateway.Application.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Extracts the last four digits of a card number.
        /// </summary>
        public static int GetLastFourDigits(this string cardNumber)
        {
            return string.IsNullOrEmpty(cardNumber) ? throw new ArgumentNullException(nameof(cardNumber)) : Convert.ToInt32(cardNumber[^4..]);
            //new string('*', cardNumber.Length - 4) + lastFourDigits;
        }
    }
}
