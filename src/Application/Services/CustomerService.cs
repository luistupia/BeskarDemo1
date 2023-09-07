using Application.Common.Enum;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mapping;
using Application.Common.Wrappers;
using Common.Extensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dtos;
using Models.Requests;

namespace Application.Services;

internal class CustomerService : ICustomerService
{
    #region Variables

    private readonly ICustomerRepository _customerRepository;
    private readonly INorthwindContext _northwindContext;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;
    private readonly ICsvFileBuilder _csvFileBuilder;
    private readonly IExcelFileBuilder _excelFileBuilder;
    #endregion

    #region Constructor

    public CustomerService(ICustomerRepository customerRepository,
        INorthwindContext northwindContext, IMapper mapper, ILogger<CustomerService> logger,
        ICsvFileBuilder csvFileBuilder, IExcelFileBuilder excelFileBuilder)
    {
        _customerRepository = customerRepository;
        _northwindContext = northwindContext;
        _mapper = mapper;
        _logger = logger;
        _csvFileBuilder = csvFileBuilder;
        _excelFileBuilder = excelFileBuilder;
    }

    #endregion

    #region Commands

    /// <summary>
    /// Crear un cliente
    /// </summary>
    /// <param name="request">Objeto tipo CreateCustomerRequest</param>
    /// <returns>ResponseService.Ok si se ejecutó correctamente</returns>
    public async Task<ResponseService<bool>> CreateAsync(CreateCustomerRequest request)
    {
        var existEntity = await _northwindContext.Customers.FindAsync(request.CustomerID);
        if (existEntity is not null)
            throw new DuplicateKeyException("Customer", request.CustomerID!);

        var entity = _mapper.Map<CreateCustomerRequest, Customer>(request);
        _northwindContext.Customers.Add(entity);
        await _northwindContext.SaveChangesAsync();
        return ResponseService<bool>.Ok(true, "Cliente agregado");
    }

    /// <summary>
    /// Actualizar un cliente
    /// </summary>
    /// <param name="customerId">Identificador del cliente</param>
    /// <param name="request">Objeto UpdateCustomerRequest con los valores a modificar</param>
    /// <returns>ResponseService.Ok si se ejecutó correctamente</returns>
    public async Task<ResponseService<bool>> UpdateAsync(string customerId, UpdateCustomerRequest request)
    {
        //Usando ExecuteUpdateAsync, EF >=7 
        var row = await _northwindContext.Customers
            .Where(x => x.CustomerID == customerId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(b => b.CompanyName, request.CompanyName)
                    .SetProperty(b => b.ContactName, request.ContactName)
                    .SetProperty(b => b.Address, request.Address));

        if (row == 0)
            throw new NotFoundException($"No se actualizó ningún cliente, {customerId}");

        return ResponseService<bool>.Ok(true, "Cliente modificado");
    }

    #endregion

    #region Queries

    /// <summary>
    /// Devuelve todos los registros
    /// </summary>
    /// <returns>Listado de clientes</returns>
    public async Task<ResponseService<List<CustomerDto>?>> GetAllAsync()
    {
        var result = await _northwindContext.Customers.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<Customer>, List<CustomerDto>>(result);
        return result.Any()
            ? ResponseService<List<CustomerDto>>.Ok(dto)
            : ResponseService<List<CustomerDto>>.Error("No existe registros");
    }

    /// <summary>
    /// Devuelve los clientes paginado
    /// </summary>
    /// <param name="page">numero de pagina</param>
    /// <param name="size">cantidad registros por pagina</param>
    /// <param name="sortOrder">ordenar por</param>
    /// <returns></returns>
    public async Task<(ResponseService<List<CustomerDto>?> records, int rowCount, int totalPages)> GetAllPagingAsync(int page, int size, string? sortOrder)
    {
        //var result = await _northwindContext.Customers.AsNoTracking().GetPagedAsync(page, size);
        IQueryable<Customer> query = sortOrder switch
        {
            "createdAt_desc" => _northwindContext.Customers.OrderByDescending(x => x.CreatedAt),
            "createdAt" => _northwindContext.Customers.OrderBy(x => x.CreatedAt),
            _ => _northwindContext.Customers.OrderBy(x => x.CreatedAt)
        };
        var result = await query.AsNoTracking().GetPagedAsync(page, size);
        var dto = _mapper.Map<List<Customer>, List<CustomerDto>>(result.Results.ToList());
        return (ResponseService<List<CustomerDto>>.Ok(dto), result.RowCount, result.TotalPages);
    }

    /// <summary>
    /// Devuelve cliente por identificador
    /// </summary>
    /// <param name="customerId">identificador del cliente</param>
    /// <returns>Información del cliente</returns>
    public async Task<ResponseService<CustomerDto?>> FindByIdAsync(string customerId)
    {
        //Valida si existe el cliente en cache
        //Si existe retorna el cliente almacenado en memoria
        /*var keyCache = $"customer-{customerId}";
        if (_cacheService.TryGet<Customer?>(keyCache, out var customer))
        {
            var dtoCache = _mapper.Map<Customer, CustomerDto>(customer!);
            return ResponseService<CustomerDto?>.Ok(dtoCache);
        }*/

        var result = await _northwindContext.Customers.FindAsync(customerId);
        if (result is null)
        {
            _logger.LogWarning($"No existe cliente : {customerId}");
            throw new NotFoundException($"No existe cliente {customerId}");
        }

        var dto = _mapper.Map<Customer, CustomerDto>(result);
        //await _cacheService.SetAsync<CustomerDto?>(keyCache, dto);
        return ResponseService<CustomerDto?>.Ok(dto);
    }

    /// <summary>
    /// Total productos comprados por cliente
    /// </summary>
    /// <param name="customerId">identificador del cliente</param>
    /// <returns>Listado de productos</returns>
    public async Task<ResponseService<List<CustomerOrderHistDto>?>> CustomerOrderHistAsync(string customerId)
    {
        var result = await _customerRepository.CustomerOrderHistAsync(customerId);
        return !result.Any()
            ? ResponseService<List<CustomerOrderHistDto>>.Error("No existe registros")
            : ResponseService<List<CustomerOrderHistDto>>.Ok(result.ToList());
    }

    /// <summary>
    /// Devuelve un tipo de archivo con los productos comprados por un cliente
    /// </summary>
    /// <param name="customerId">identificador cliente</param>
    /// <param name="fileType">tipo de archivo</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException">No existe registros</exception>
    /// <exception cref="ArgumentOutOfRangeException">Fuera de rango</exception>
    public async Task<ExportQuery> ExportFileCustomerOrderHistToAsync(string customerId, FileType fileType)
    {
        var result = await CustomerOrderHistAsync(customerId);
        if (result.Result is null)
            throw new NotFoundException($"Método: ExportFileCustomerOrderHistToAsync,\n " +
                                        $"Mensaje: No existe registros {customerId}");

        var exportQuery = fileType switch
        {
            FileType.Excel => new ExportQuery
            {
                Content = _excelFileBuilder.GetStreamFileExcel(result.Result, string.Empty),
                FileName = $"{Guid.NewGuid().ToString()}.xlsx",
                ContentType = "application/vnd.ms-excel"
            },
            FileType.Csv => new ExportQuery
            {
                Content = _csvFileBuilder.GetStreamFileCsv(result.Result),
                FileName = $"{Guid.NewGuid().ToString()}.csv",
                ContentType = "application/csv"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
        };
        return exportQuery;
    }

    #endregion
}