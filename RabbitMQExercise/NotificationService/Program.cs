using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Espera inicial para asegurar que el contenedor de RabbitMQ esté listo
await Task.Delay(5000);

var factory = new ConnectionFactory() { HostName = "rabbitmq" };

// Métodos asíncronos en RabbitMQ.Client v7+
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "orders_queue",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

Console.WriteLine("[NotificationService] Esperando mensajes de pedidos...");

// En la v7 se utiliza AsyncEventingBasicConsumer
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[NotificationService] Evento recibido: {message}");
    Console.WriteLine($" --> Procesando envío de correo de notificación...");
    
    await Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: "orders_queue",
                                autoAck: true,
                                consumer: consumer);

Console.WriteLine("Presiona CTRL+C para salir...");
await Task.Delay(Timeout.Infinite);