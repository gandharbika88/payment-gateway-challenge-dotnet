using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.BankSimulator;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Infrastructure
{
    /// <summary>
    /// Adapter for communicating with the bank.
    /// </summary>
    public class BankAdapter(HttpClient httpClient, ILogger<BankAdapter> logger) : IBankAdapter
    {
        private const string BankUrl = "http://localhost:8080/payments";
        private static readonly MediaTypeWithQualityHeaderValue JsonMediaType = new("application/json");

        /// <inheritdoc/>
        public async Task<BankResponse> ProcessPaymentAsync(BankRequest bankRequest)
        {
            try
            {
                string jsonRequest = JsonSerializer.Serialize(bankRequest);
                HttpRequestMessage requestMessage = new(HttpMethod.Post, BankUrl)
                {
                    Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
                };

                EnsureJsonAcceptHeader(httpClient);

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var bankResponse = JsonSerializer.Deserialize<BankResponse>(responseContent) ?? new BankResponse { Authorized = false, AuthorizationCode = null, ProcessingStatus = BankProcessingStatus.Failure };
                    logger.LogInformation("Payment processed successfully.");
                    return bankResponse with { ProcessingStatus = bankResponse.Authorized ? BankProcessingStatus.Success : BankProcessingStatus.Failure };
                }
                else
                {
                    logger.LogWarning("Payment processing failed with status code {statusCode}.", response.StatusCode);
                    return new BankResponse { Authorized = false, AuthorizationCode = null, ProcessingStatus = BankProcessingStatus.Failure };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the payment.");
                return new BankResponse { Authorized = false, AuthorizationCode = null, ProcessingStatus = BankProcessingStatus.InternalError };
            }
        }

        private static void EnsureJsonAcceptHeader(HttpClient httpClient)
        {
            if (!httpClient.DefaultRequestHeaders.Accept.Contains(JsonMediaType))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(JsonMediaType);
            }
        }
    }
}
