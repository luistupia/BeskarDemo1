using System.Data;

namespace Infraestructure.Common.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection NorthwindDbConnection();
}