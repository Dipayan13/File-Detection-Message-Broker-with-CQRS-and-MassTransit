using DMS.DomainEvents;
using DocumentListenerService.Data;
using DocumentListenerService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using Microsoft.Extensions.Hosting;   
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;
    private readonly IFileSystem _fileSystem;
    private readonly IDocumentIdGenerator _documentIdGenerator;
    private readonly IFileSourceIdentifier _fileSourceIdentifier;
    private readonly ICorrelationIdGenerator _correlationIdGenerator;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceSettings _serviceSettings;
    private readonly string _monitorFolder = @"D:\MonitoredFolder";

    public Worker(ILogger<Worker> logger, IBus bus, IFileSystem fileSystem, IDocumentIdGenerator documentIdGenerator,
                  IFileSourceIdentifier fileSourceIdentifier, ICorrelationIdGenerator correlationIdGenerator, IServiceProvider serviceProvider, IOptions<ServiceSettings> serviceSettingsOptions)
    {
        _logger = logger;
        _bus = bus;
        _fileSystem = fileSystem;
        _documentIdGenerator = documentIdGenerator;
        _fileSourceIdentifier = fileSourceIdentifier;
        _correlationIdGenerator = correlationIdGenerator;
        _serviceProvider = serviceProvider;
        _serviceSettings = serviceSettingsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_fileSystem.Directory.Exists(_monitorFolder))
        {
            _fileSystem.Directory.CreateDirectory(_monitorFolder);
        }

        var watcher = new FileSystemWatcher(_monitorFolder)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
            Filter = "*.pdf"
        };

        watcher.Created += async (sender, e) =>
        {
            var documentId = _documentIdGenerator.GenerateUniqueDocumentId();
            var correlationId = _correlationIdGenerator.GenerateCorrelationId();
            var source = _fileSourceIdentifier.IdentifySource();

            var base64Content = Convert.ToBase64String(await _fileSystem.File.ReadAllBytesAsync(e.FullPath));

            var timeStamp = DateTime.UtcNow;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();

                var documentRecord = new DocumentRecord
                {
                    id = Guid.Parse(documentId),
                    file_name = e.Name,
                    file_base64 = base64Content,
                    time_stamp = timeStamp
                };

                try
                {
                    dbContext.documents.Add(documentRecord);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Failed to save document record to the database.");
                    return;
                }
            }

            var message = new DocumentReceivedMessage
            {
                DocumentId = documentId,
                Source = source,
                DocumentSrc = $"https://localhost:7023/api/files/download/{documentId}"
            };

            var documentReceivedEvent = new DocumentReceived
            {
                Context = new Context
                {
                    TimeStamp = timeStamp.ToString("o"),
                    CorrelationId = correlationId,
                    SourceServiceName = _serviceSettings.SourceServiceName,
                    Domain = _serviceSettings.Domain,
                    CompanyName = _serviceSettings.CompanyName
                },
                Message = message
            };

            await _bus.Publish(documentReceivedEvent);
            _logger.LogInformation($"New file detected: {e.Name}. Document ID: {documentId}");
        };

        watcher.EnableRaisingEvents = true;

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
