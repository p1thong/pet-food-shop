using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll([FromQuery] string? status = null)
    {
        var payments = status != null
            ? await _paymentService.GetPaymentsByStatusAsync(status)
            : await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        return Ok(payment);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<PaymentDto>> GetByOrderId(int orderId)
    {
        var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto createDto)
    {
        var payment = await _paymentService.CreatePaymentAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<PaymentDto>> Update(int id, [FromBody] UpdatePaymentDto updateDto)
    {
        var payment = await _paymentService.UpdatePaymentAsync(id, updateDto);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        return Ok(payment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _paymentService.DeletePaymentAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Payment not found" });
        }

        return NoContent();
    }
}

