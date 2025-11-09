using System.Text;
using Application;
using WebApi.Config;
using Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.AddServices();

var app = builder.Build();

app.Configure();


app.Run();