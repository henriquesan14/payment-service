using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Payment.API.Mongo;
using Payment.API.Schemas;

namespace Payment.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<PaymentSchema> _paymentsCollection;
        public PaymentRepository(IOptions<PaymentDatabaseSettings> bookStoreDatabaseSettings) {
            var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _paymentsCollection = mongoDatabase.GetCollection<PaymentSchema>(
                bookStoreDatabaseSettings.Value.PaymentsCollectionName);
        }
        public async Task CreateAsync(PaymentSchema payment)
        {
            await _paymentsCollection.InsertOneAsync(payment);
        }
    }
}
