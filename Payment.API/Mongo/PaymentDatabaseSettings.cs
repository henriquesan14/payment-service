namespace Payment.API.Mongo
{
    public class PaymentDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PaymentsCollectionName { get; set; } = null!;
    }
}
