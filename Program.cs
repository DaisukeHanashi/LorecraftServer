using Lorecraft_API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration; 
var env = builder.Environment; 
// Add services to the container.

builder.Logging
.AddConfiguration(config.GetSection("Logging"))
.AddConsole()
.AddDebug(); 

builder.Services.InstallBaseService(config, env); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRunners(app.Environment); 

app.MapControllers();


app.Run();
