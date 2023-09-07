using Application.Common.Enum;
using Application.Common.Interfaces;
using Application.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Requests;
using Newtonsoft.Json;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;
    //private readonly IValidator<CreateCustomerRequest> _validator;
    public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
    {
        _logger = logger;
        _customerService = customerService;
    }


    [HttpGet("demo")]
    public IActionResult Demo(){
        return Ok("test");
    }

    /// <summary>
    /// Muestra todos los clientes
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseService<List<CustomerDto>>))]
    public async Task<IActionResult> GetAll()
    {
        var result = await _customerService.GetAllAsync();
        _logger.LogInformation($"Total clientes : {result.Result?.Count}");
        return Ok(result);
    }

    /// <summary>
    /// Muestra todos los clientes utilizando paginación
    /// </summary>
    /// <param name="page">numero pagina</param>
    /// <param name="size">total registros por pagina</param>
    /// <param name="order">ordernado por</param>
    /// <returns></returns>
    [HttpGet, Route("show")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDataTabulator<ResponseService<List<CustomerDto>>>))]
    public async Task<IActionResult> GetAll(int page, int size, string? order)
    {
        var (records, rowCount, totalPages) = await _customerService.GetAllPagingAsync(page, size, order);
        var response = new ResponseDataTabulator<ResponseService<List<CustomerDto>?>> { TotalPages = totalPages, Total = rowCount, Data = records };
        return Ok(response);
    }

    /// <summary>
    /// Muestra un cliente por identificador
    /// </summary>
    /// <param name="customerId">identificador de cliente</param>
    /// <returns></returns>
    [HttpGet, Route("{customerId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseService<CustomerDto>))]
    public async Task<IActionResult> FindById(string customerId)
    {
        var result = await _customerService.FindByIdAsync(customerId);
        //return result.Result is null ? result.ToJsonNotFoundResult() : result.ToJsonResult();
        return result.Result is null ? NotFound(result) : Ok(result);
    }

    /// <summary>
    /// Acción para crear un cliente
    /// </summary>
    /// <param name="request">Parametros de entrada</param>
    /// <returns>Modelo creado</returns>
    /// <response code="200">Devuelve el modelo creado</response>
    /// <response code="400">En caso de exisitr un error en validación</response>
    /// <response code="409">Devuelve cuando ya existe un modelo con el mismo identificador</response>
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseService<bool>))]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        /*var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return validation.ToErrorsResult();*/
        var result = await _customerService.CreateAsync(request);
        if (result.SuccessResult)
            _logger.LogInformation($"Cliente registrado, {JsonConvert.SerializeObject(request)}");

        return Ok(result);
        /*return new CreatedAtActionResult(nameof(FindById), 
            "Customer", 
            new { customerId = request.CustomerID },request);*/
    }

    /// <summary>
    /// Exporta los productos comprados por cliente
    /// </summary>
    /// <param name="customerId">identificador de cliente</param>
    /// <returns></returns>
    [HttpPost, Route("{customerId}/ExportCustomerOrderHist")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
    public async Task<IActionResult> ExportCustomerOrderHist(string customerId)
    {
        var result = await _customerService.ExportFileCustomerOrderHistToAsync(customerId, FileType.Excel);
        return File(result.Content!, result.ContentType!, result.FileName);
    }

    /// <summary>
    /// Actualizar un cliente
    /// </summary>
    /// <param name="customerId">identificador de cliente</param>
    /// <param name="request">Parametros de entrada</param>
    /// <returns></returns>
    [HttpPut, Route("{customerId}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseService<bool>))]
    public async Task<IActionResult> Update(string customerId, [FromBody] UpdateCustomerRequest request) => Ok(await _customerService.UpdateAsync(customerId, request));
}