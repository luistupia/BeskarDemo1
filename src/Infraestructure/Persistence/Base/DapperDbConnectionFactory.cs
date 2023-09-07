using System.Data;
using Infraestructure.Common.Enums;
using Infraestructure.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infraestructure.Persistence.Base;

public class DapperDbConnectionFactory : IDisposable,IDbConnectionFactory
{
    private readonly IDictionary<DatabaseConnectionName, string> _connectionDict;

    public DapperDbConnectionFactory(IDictionary<DatabaseConnectionName, string> connectionDict) => _connectionDict = connectionDict;

    public IDbConnection NorthwindDbConnection() => CreateDbConnection(DatabaseConnectionName.NorthwindDbConnection);
    
    private IDbConnection CreateDbConnection(DatabaseConnectionName connectionName)
    {
        if (_connectionDict.TryGetValue(connectionName, out var connectionString))
            return new SqlConnection(connectionString);

        throw new ArgumentNullException("No existe cadena de conexión");
    }

    public void Dispose()
    {
        NorthwindDbConnection().Dispose();
    }
}