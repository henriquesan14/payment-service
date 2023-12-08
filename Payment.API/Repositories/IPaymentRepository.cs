using Payment.API.Schemas;

namespace Payment.API.Repositories
{
    public interface IPaymentRepository
    {
        Task CreateAsync(PaymentSchema payment);
    }
}
