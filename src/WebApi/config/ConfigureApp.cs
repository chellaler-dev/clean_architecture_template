using WebApi.Endpoints;

namespace WebApi.Config;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapGet("/", () => "Welcome to clean architecture template");

        app.MapUserEndpoints();
    }

}