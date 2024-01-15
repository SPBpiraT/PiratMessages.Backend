using PiratMessages.Persistence;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<MessagesDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception exception)
    {
       
    }
}

app.Run();
