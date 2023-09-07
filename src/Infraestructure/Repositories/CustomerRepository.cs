using System.Data;
using Dapper;
using Domain.Interfaces;
using Infraestructure.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Models.Dtos;

namespace Infraestructure.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public CustomerRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    /// <summary>
    /// Total productos comprados por cliente
    /// </summary>
    /// <param name="customerId">identificador del cliente</param>
    /// <returns>Listado de productos</returns>
    public async Task<IEnumerable<CustomerOrderHistDto>> CustomerOrderHistAsync(string customerId)
    {
        using var connection = _connectionFactory.NorthwindDbConnection();
        var result = await connection.QueryAsync<CustomerOrderHistDto>("CustOrderHist",
            new
            {
                CustomerID = customerId
            }, commandType: CommandType.StoredProcedure);

        return result;
    }

}