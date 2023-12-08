using MongoDB.Bson.Serialization.Attributes;

namespace Payment.API.Schemas
{
    public class PaymentSchema
    {
        public PaymentSchema(int orderId, string creditCardNumber, string cvv, string expiresAt, string fullName, decimal amount)
        {
            OrderId = orderId;
            CreditCardNumber = creditCardNumber;
            Cvv = cvv;
            ExpiresAt = expiresAt;
            FullName = fullName;
            Amount = amount;
        }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public int OrderId { get; set; }
        public string CreditCardNumber { get; set; }
        public string Cvv { get; set; }
        public string ExpiresAt { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
    }
}
