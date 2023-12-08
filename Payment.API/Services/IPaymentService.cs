using Payment.API.Models;

namespace Payment.API.Services
{
    public interface IPaymentService
    {
        void ProcessPayment(PaymentInfoInputModel paymentInfo);
    }
}
