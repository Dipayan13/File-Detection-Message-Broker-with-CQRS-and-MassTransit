using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<DocumentReceivedConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h => { });

                        // No explicit queue name configuration
                        cfg.ConfigureEndpoints(context); 
                    });
                });

                services.AddHostedService<MassTransitConsoleHostedService>();
            })
            .Build();

        await host.RunAsync();
    }
}

public class MassTransitConsoleHostedService : IHostedService
{
    private readonly IBusControl _busControl;

    public MassTransitConsoleHostedService(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _busControl.StartAsync(cancellationToken);
        Console.WriteLine("MassTransit Bus started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _busControl.StopAsync(cancellationToken);
        Console.WriteLine("MassTransit Bus stopped.");
    }
}
