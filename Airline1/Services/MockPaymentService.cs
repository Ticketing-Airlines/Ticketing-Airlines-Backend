namespace Airline1.Services
{
    public class MockPaymentService : IService.IPaymentService
    {
        public async Task<string> ProcessPaymentAsync(decimal amount, string currency, string paymentMethod, string reference)
        {
            
            await Task.Delay(500);

            
            var supportedMethods = new[] { "GCash", "PayMaya", "CreditCard" };
            if (!supportedMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Payment method '{paymentMethod}' is not supported.");
            }

           
            return $"MOCK-{paymentMethod.ToUpper()}-{reference}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}
