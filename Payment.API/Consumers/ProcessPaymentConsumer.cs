using Payment.API.Models;
using Payment.API.Repositories;
using Payment.API.Schemas;
using Payment.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Payment.API.Consumers
{
    public class ProcessPaymentConsumer : BackgroundService
    {
        private const string QUEUE = "Payments";
        private const string PAYMENT_APPROVED_QUEUE = "PaymentsApproved";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        public ProcessPaymentConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = configuration["MessageBus:HostName"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: PAYMENT_APPROVED_QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                var paymentInfoJson = Encoding.UTF8.GetString(byteArray);

                var paymentInfo = JsonSerializer.Deserialize<PaymentInfoInputModel>(paymentInfoJson);

                await ProcessPayment(paymentInfo!);

                var paymentApproved = new PaymentApprovedIntegrationEvent(paymentInfo!.OrderId);
                var paymentApprovedJson = JsonSerializer.Serialize(paymentApproved);
                var paymentApprovedBytes = Encoding.UTF8.GetBytes(paymentApprovedJson);

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: PAYMENT_APPROVED_QUEUE,
                    basicProperties: null,
                    body: paymentApprovedBytes
                    );

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(QUEUE, false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessPayment(PaymentInfoInputModel paymentInfo)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                paymentService.ProcessPayment(paymentInfo);

                var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                var schema = new PaymentSchema(paymentInfo.OrderId, paymentInfo.CreditCardNumber, paymentInfo.Cvv, paymentInfo.ExpiresAt, paymentInfo.FullName, paymentInfo.Amount);
                await paymentRepository.CreateAsync(schema);
            }
        }
    }
}
