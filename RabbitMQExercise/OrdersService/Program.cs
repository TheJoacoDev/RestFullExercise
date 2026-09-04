using System.Text;
using RabbitMQ.Client;

// Espera inicial para asegurar que el contenedor de RabbitMQ esté listo
await Task.Delay(5000);

var factory = new ConnectionFactory() { HostName = "rabbitmq" };

// En las versiones recientes se usan métodos asíncronos (Async)
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declaración de la cola de pedidos
await channel.QueueDeclareAsync(queue: "orders_queue",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

for (int i = 1; i <= 5; i++)
{
    string orderEvent = $"{{\"OrderId\": {i}, \"Product\": \"P100\", \"Email\": \"cliente{i}@correo.com\"}}";
    var body = Encoding.UTF8.GetBytes(orderEvent);

    await channel.BasicPublishAsync(exchange: "",
                                    routingKey: "orders_queue",
                                    mandatory: false,
                                    body: body);

    Console.WriteLine($"[OrdersService] Pedido Creado enviado: {orderEvent}");
    await Task.Delay(1000);
}

Console.WriteLine("[OrdersService] Finalizó el envío de pedidos.");