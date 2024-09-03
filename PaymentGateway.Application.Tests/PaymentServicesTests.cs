using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Application.BankSimulator;
using PaymentGateway.Application.Repository;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Application.Tests
{
    public class PaymentServicesTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IBankAdapter> _bankAdapterMock;
        private readonly Mock<ILogger<PaymentServices>> _loggerMock;
        private readonly PaymentServices _paymentServices;

        public PaymentServicesTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _bankAdapterMock = new Mock<IBankAdapter>();
            _loggerMock = new Mock<ILogger<PaymentServices>>();
            _paymentServices = new PaymentServices(_paymentRepositoryMock.Object, _bankAdapterMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddPaymentAsyncValidPaymentReturnsAuthorizedResponse()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = 2222405343248877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "GBP",
                Cvv = 123
            };

            var bankResponse = new BankResponse
            {
                Authorized = true,
                AuthorizationCode = "0bb07405-6d44-4b50-a14f-7ae0beff13ad",
                ProcessingStatus = BankProcessingStatus.Success
            };

            _bankAdapterMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<BankRequest>())).ReturnsAsync(bankResponse);

            // Act
            var result = await _paymentServices.AddPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(PaymentStatus.Authorized, result.Status);
            Assert.Equal(8877, result.CardNumberLastFour);
            Assert.Equal(4, result.ExpiryMonth);
            Assert.Equal(2025, result.ExpiryYear);
            Assert.Equal(100, result.Amount);
            Assert.Equal("GBP", result.Currency);
            _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PostPaymentResponse>()), Times.Once);
        }

        [Fact]
        public async Task AddPaymentAsyncInvalidCvvReturnsRejectedResponse()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = 2222405343248877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "GBP",
                Cvv = 333
            };

            // Act
            var result = await _paymentServices.AddPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(PaymentStatus.Rejected, result.Status);
            _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PostPaymentResponse>()), Times.Never);
        }

        [Fact]
        public async Task AddPaymentAsyncCardDetailsReturnsDeclinedResponse()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = 2222405343248112,
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Amount = 60000,
                Currency = "USD",
                Cvv = 456
            };
            var bankResponse = new BankResponse
            {
                Authorized = false,
                AuthorizationCode = null,
                ProcessingStatus = BankProcessingStatus.Failure
            };
            _bankAdapterMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<BankRequest>())).ReturnsAsync(bankResponse);

            // Act
            var result = await _paymentServices.AddPaymentAsync(paymentRequest);

            // Assert
            // Assert
            Assert.Equal(PaymentStatus.Declined, result.Status);
            Assert.Equal(8112, result.CardNumberLastFour);
            Assert.Equal(1, result.ExpiryMonth);
            Assert.Equal(2026, result.ExpiryYear);
            Assert.Equal(60000, result.Amount);
            Assert.Equal("USD", result.Currency);
            _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PostPaymentResponse>()), Times.Once);
        }

        [Fact]
        public async Task GetPaymentAsyncValidIdReturnsPaymentResponse()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var paymentResponse = new GetPaymentResponse
            {
                Id = paymentId,
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "GBP"
            };

            _paymentRepositoryMock.Setup(x => x.GetAsync(paymentId)).ReturnsAsync(paymentResponse);

            // Act
            var result = await _paymentServices.GetPaymentAsync(paymentId);

            // Assert
            Assert.Equal(paymentId, result.Id);
            Assert.Equal(PaymentStatus.Authorized, result.Status);
            Assert.Equal(8877, result.CardNumberLastFour);
            Assert.Equal(4, result.ExpiryMonth);
            Assert.Equal(2025, result.ExpiryYear);
            Assert.Equal(100, result.Amount);
            Assert.Equal("GBP", result.Currency);
        }
    }
}
