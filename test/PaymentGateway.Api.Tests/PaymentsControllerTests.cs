using System.Net;

using Microsoft.AspNetCore.Mvc;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly PaymentsController _paymentsController;
    private readonly Mock<IPaymentServices> _paymentServicesMock;

    public PaymentsControllerTests()
    {
        _paymentServicesMock = new Mock<IPaymentServices>();
        _paymentsController = new PaymentsController(_paymentServicesMock.Object);
    }

    [Fact]
    public async Task Returns200IfPaymentSuccessful()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentResponse = new GetPaymentResponse
        {
            Id = paymentId,
            ExpiryYear = _random.Next(2024, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };

        _paymentServicesMock.Setup(service => service.GetPaymentAsync(paymentId)).ReturnsAsync(paymentResponse);

        // Act
        var result = await _paymentsController.GetPaymentAsync(paymentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPayment = Assert.IsType<GetPaymentResponse>(okResult.Value);
        Assert.Equal(paymentResponse.Id, returnedPayment.Id);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var problemDetails = new ProblemDetails
        {
            Title = "Payment not found",
            Status = (int)HttpStatusCode.NotFound,
            Detail = $"Payment with id {paymentId} not found."
        };

        _paymentServicesMock.Setup(service => service.GetPaymentAsync(paymentId))
            .ThrowsAsync(new KeyNotFoundException($"Payment with id {paymentId} not found."));

        // Act
        var result = await _paymentsController.GetPaymentAsync(paymentId);

        // Assert
        var objectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        var returnedProblemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(problemDetails.Title, returnedProblemDetails.Title);
        Assert.Equal(problemDetails.Status, returnedProblemDetails.Status);
        Assert.Equal(problemDetails.Detail, returnedProblemDetails.Detail);
    }
}