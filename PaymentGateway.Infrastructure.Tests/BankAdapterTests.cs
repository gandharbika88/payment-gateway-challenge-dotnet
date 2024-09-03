using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using PaymentGateway.Application.BankSimulator;

using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Infrastructure.Tests
{
    public class BankAdapterTests
    {
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;
        private readonly Mock<ILogger<BankAdapter>> _loggerMock;
        private readonly BankAdapter _bankAdapter;

        public BankAdapterTests()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _httpClientMock = new Mock<HttpClient>(_httpHandlerMock.Object);
            _loggerMock = new Mock<ILogger<BankAdapter>>();
            _bankAdapter = new BankAdapter(_httpClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsyncSuccess()
        {
            // Arrange
            var bankRequest = new BankRequest
            {
                CardNumber = "2222405343248877",
                ExpiryDate = "04/2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = 123
            };
            var bankResponse = new BankResponse { Authorized = true, AuthorizationCode = "0bb07405-6d44-4b50-a14f-7ae0beff13ad", ProcessingStatus = BankProcessingStatus.Success };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(bankResponse), Encoding.UTF8, "application/json")
            };

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage).Verifiable();

            // Act
            var result = await _bankAdapter.ProcessPaymentAsync(bankRequest);

            // Assert
            Assert.Equal(BankProcessingStatus.Success, result.ProcessingStatus);
            Assert.True(result.Authorized);
            Assert.Equal("0bb07405-6d44-4b50-a14f-7ae0beff13ad", result.AuthorizationCode);
        }

        [Fact]
        public async Task ProcessPaymentAsyncFailure()
        {
            // Arrange
            var bankRequest = new BankRequest
            {
                CardNumber = "2222405343248877",
                ExpiryDate = "04/2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = 123
            };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _httpHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(httpResponseMessage).Verifiable();

            // Act
            var result = await _bankAdapter.ProcessPaymentAsync(bankRequest);

            // Assert
            Assert.Equal(BankProcessingStatus.Failure, result.ProcessingStatus);
            Assert.False(result.Authorized);
            Assert.Null(result.AuthorizationCode);
        }

        [Fact]
        public async Task ProcessPaymentAsyncFailureDueToInvalidDetails()
        {
            // Arrange
            var bankRequest = new BankRequest
            {
                CardNumber = "2222405343248112",
                ExpiryDate = "01/2026",
                Amount = 60000,
                Currency = "USD",
                Cvv = 456
            };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _httpHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(httpResponseMessage).Verifiable();

            // Act
            var result = await _bankAdapter.ProcessPaymentAsync(bankRequest);

            // Assert
            Assert.Equal(BankProcessingStatus.Failure, result.ProcessingStatus);
            Assert.False(result.Authorized);
            Assert.Null(result.AuthorizationCode);
        }

        [Fact]
        public async Task ProcessPaymentAsyncException()
        {
            // Arrange
            var bankRequest = new BankRequest
            {
                CardNumber = "2222405343248877",
                ExpiryDate = "04/2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = 123
            };

            _httpHandlerMock.Protected()
              .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
              .ThrowsAsync(new Exception("Network error")).Verifiable();

            // Act
            var result = await _bankAdapter.ProcessPaymentAsync(bankRequest);

            // Assert
            Assert.Equal(BankProcessingStatus.InternalError, result.ProcessingStatus);
            Assert.False(result.Authorized);
            Assert.Null(result.AuthorizationCode);
        }
    }
}
