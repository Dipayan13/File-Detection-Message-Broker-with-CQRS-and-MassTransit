using MassTransit;
using DMS.DomainEvents;
using System.Text.Json;

public class DocumentReceivedConsumer : IConsumer<DocumentReceived>
{
    public Task Consume(ConsumeContext<DocumentReceived> context)
    {
        // Deserialization
        var messageJson = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("Received Document Message:");
        Console.WriteLine(messageJson);

        return Task.CompletedTask;
    }
}
