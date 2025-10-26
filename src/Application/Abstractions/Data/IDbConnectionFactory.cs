using System.Data;

namespace Application.Abstractions.Data;


// The interface is abstract, so the application layer does not care about the specific database provider.

// GetOpenConnection() is expected to return a ready-to-use IDbConnection.

// This is used for Dapper, which works directly with IDbConnection.

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
}
