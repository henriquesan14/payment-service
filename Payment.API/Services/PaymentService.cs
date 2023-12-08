using Payment.API.Models;

namespace Payment.API.Services
{
    public class PaymentService : IPaymentService
    {
        public void ProcessPayment(PaymentInfoInputModel paymentInfo)
        {
            Console.WriteLine("Processou!");
        }
    }
}
