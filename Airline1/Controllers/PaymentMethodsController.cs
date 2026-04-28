using Microsoft.AspNetCore.Mvc;
using Airline1.IService;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    public class PaymentMethodsController(IPaymentMethodService service) : ControllerBase
    {
        private readonly IPaymentMethodService _service = service;

        // GET api/payment-methods/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMethods([FromQuery] string? category, [FromQuery] bool? featured)
        {
            var result = await _service.GetActiveMethodsAsync(category, featured);

            if (result.PaymentMethods.Count == 0)
            {
                return Ok(new
                {
                    success = false,
                    error = "NO_PAYMENT_METHODS",
                    message = "No active payment methods available at this time"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // GET api/payment-methods/faqs
        [HttpGet("faqs")]
        public async Task<IActionResult> GetFAQs()
        {
            var result = await _service.GetFAQsAsync();
            return Ok(new
            {
                success = true,
                data = new
                {
                    faqs = result,
                    total = result.Count
                }
            });
        }
    }
}
