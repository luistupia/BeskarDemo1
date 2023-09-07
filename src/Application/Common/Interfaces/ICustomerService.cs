using Application.Common.Enum;
using Application.Common.Wrappers;
using Models.Dtos;
using Models.Requests;

namespace Application.Common.Interfaces;

public interface ICustomerService
{
    #region Queries
    Task<ResponseService<List<CustomerDto>?>> GetAllAsync();
    Task<(ResponseService<List<CustomerDto>?> records, int rowCount,int totalPages)> GetAllPagingAsync(int page, int size,string? sortOrder = null);
    Task<ResponseService<CustomerDto?>> FindByIdAsync(string customerId);
    Task<ResponseService<List<CustomerOrderHistDto>?>> CustomerOrderHistAsync(string customerId);
    Task<ExportQuery> ExportFileCustomerOrderHistToAsync(string customerId, FileType fileType);
    #endregion

    #region Commands
    Task<ResponseService<bool>> CreateAsync(CreateCustomerRequest request);
    Task<ResponseService<bool>> UpdateAsync(string customerId, UpdateCustomerRequest request);
    #endregion
}