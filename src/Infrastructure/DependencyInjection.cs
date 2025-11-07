using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Dapper;
using Domain.Users;
using Infrastructure.Caching;
using Infrastructure.Data;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SharedKernel;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDatabase(configuration).
        AddCaching(configuration);

    // Convenience method
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {

        string? connectionString = configuration.GetConnectionString("Database");
        Ensure.NotNullOrEmpty(connectionString);

        // Dapper
        // The concrete implementation of IDbConnectionFactory (Used for Dapper)
        services.AddSingleton<IDbConnectionFactory>(_ =>
            new DbConnectionFactory(new NpgsqlDataSourceBuilder(connectionString).Build()));

        // Whenever Dapper encounter a DateOnly property, it should use this handler
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());


        // EF Core
        // Register EF Core DbContext
        // Sets up EF Core with PostgreSQL, Configures the migrations table and naming convention.
        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());


        // Unit of Work 
        // Resolves IUnitOfWork to the ApplicationDbContext
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());


        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {

        // For in-memory caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();


        // For distributed caching (e.g., Redis)
        // string redisConnectionString = configuration.GetConnectionString("Cache")!;

        // services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

        // services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }


}
