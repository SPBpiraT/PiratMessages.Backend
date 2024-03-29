using System.Reflection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PiratMessages.Application;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Application.Interfaces;
using PiratMessages.Persistence;
using PiratMessages.WebApi.Jwt;
using PiratMessages.WebApi.Middleware;
using PiratMessages.WebApi;
using PiratMessages.WebApi.Services;
using PiratMessages.Messaging.RabbitMQ.Extensions;
using PiratMessages.Caching.Redis.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using PiratMessages.WebApi.HostedServices.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    config.AddProfile(new AssemblyMappingProfile(typeof(IMessagesDbContext).Assembly));
});

builder.Services.AddApplication();
builder.Services.AddPersistence();
builder.Services.AddControllers();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<MessagesDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWTSettings"));

var secretKey = builder.Configuration.GetSection("JWTSettings:SecretKey").Value;
var issuer = builder.Configuration.GetSection("JWTSettings:Issuer").Value;
var audience = builder.Configuration.GetSection("JWTSettings:Audience").Value;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = signingKey,
            ValidateIssuerSigningKey = true,
            LifetimeValidator = CustomJwtTokenLifetime.CustomLifetimeValidator
        };
    });

builder.Services.AddVersionedApiExplorer(options =>
    options.GroupNameFormat = "'v'VVV");
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>,
    ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplicationHostedServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRabbitMQ();
builder.Services.AddRedis();

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
        Log.Fatal(exception, "An error occurred while app initialization");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            config.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());

            config.RoutePrefix = String.Empty;
        }
    });
}

app.UseCustomExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseApiVersioning();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
