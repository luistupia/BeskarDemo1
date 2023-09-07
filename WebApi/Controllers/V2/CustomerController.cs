using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class CustomerController : ControllerBase
{
    [HttpGet,Route("")]
    [MapToApiVersion("2.0")]
    public IActionResult Get()
    {
        return Ok("Version 2.0");
    }
}