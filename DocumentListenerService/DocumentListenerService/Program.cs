using Microsoft.EntityFrameworkCore;
using MediatR;
using MassTransit;
using DocumentListenerService.Data;
using DocumentListenerService.Models;
using DMS.DomainEvents;
using System.Reflection;
using System.IO.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Adding services to the container
builder.Services.AddControllers();

builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));



//MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(GetDocumentByIdQueryHandler).Assembly
));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => { });
    });
});

// PostgreSQL DbContext
builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// custom services 
builder.Services.AddSingleton<IDocumentIdGenerator, DocumentIdGenerator>();
builder.Services.AddSingleton<IFileSourceIdentifier, FileSourceIdentifier>();
builder.Services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddSingleton<IFileSystem, FileSystem>();

// Hosted service (Worker)
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();

app.Run();
