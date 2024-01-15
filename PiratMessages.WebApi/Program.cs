using PiratMessages.Application;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Application.Interfaces;
using PiratMessages.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    config.AddProfile(new AssemblyMappingProfile(typeof(IMessagesDbContext).Assembly));
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

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
