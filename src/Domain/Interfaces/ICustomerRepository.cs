using Models.Dtos;

namespace Domain.Interfaces;

public interface ICustomerRepository
{
    /// <summary>
    /// Total de productos compras por cliente
    /// </summary>
    /// <param name="customerId">identificador cliente</param>
    /// <returns>Lista de productos</returns>
    Task<IEnumerable<CustomerOrderHistDto>> CustomerOrderHistAsync(string customerId);
}