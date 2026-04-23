namespace Airline1.IService
{
    public interface IPaymentService
    {
        /// <summary>
        /// Processes a mock payment for the given amount and method.
        /// Returns a transaction reference on success, throws on failure.
        /// </summary>
        Task<string> ProcessPaymentAsync(decimal amount, string currency, string paymentMethod, string reference);
    }
}
