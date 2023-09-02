using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.RabbitMqConsumer();
app.MapGet("/", () => "Hello World!");

app.Run();