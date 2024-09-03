using System.Net;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Response;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentServices paymentServices) : Controller
{
    /// <summary>
    /// Retrieves a payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>
    /// <returns>The payment details if found; otherwise, a not found result.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            GetPaymentResponse? payment = await paymentServices.GetPaymentAsync(id);
            return new OkObjectResult(payment);
        }
        catch (Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Payment not found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = ex.Message
            };
            return NotFound(problemDetails);
        }
    }

    /// <summary>
    /// Process a new payment.
    /// </summary>
    /// <param name="postPaymentRequest">The payment request details.</param>
    /// <returns>The response containing the details of the posted payment.</returns>
    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest postPaymentRequest)
    {
        PostPaymentResponse payment = await paymentServices.AddPaymentAsync(postPaymentRequest);

        return new OkObjectResult(payment);
    }
}
