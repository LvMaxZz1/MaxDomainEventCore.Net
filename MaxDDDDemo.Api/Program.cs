using Autofac.Extensions.DependencyInjection;
using LvMaxDomainEventCore.Net.AutofacDependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;

var configuration = GetConfiguration();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfiguration(configuration);
builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddMemoryCache();
builder.Services.AddOptions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder =>
{
    containerBuilder.RegisterMaxDomainEventInitiator();
    containerBuilder.RegisterMaxDomainEventInterceptor();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true);

    return builder.Build();
}