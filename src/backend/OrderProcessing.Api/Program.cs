using OrderProcessing.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiCore()                          
    .AddApiServices(builder.Configuration) 
    .AddSwaggerDocumentation();            

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseApiPipeline();

app.Run();
