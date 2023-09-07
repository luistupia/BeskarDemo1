using Application.Common.Interfaces;
using Application.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Dtos;
using Models.Requests;
using Moq;
using NSubstitute;
using WebApi.Controllers.V1;

namespace WebApi.Tests.CustomerTests;

public class CustomerControllerTest
{
    private readonly CustomerController _sut;
    private readonly Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
    private readonly ILogger<CustomerController> _logging = Substitute.For<ILogger<CustomerController>>();

    public CustomerControllerTest()
    {
        _sut = new CustomerController(_logging, _customerService.Object);
    }

    [Fact]
    public async Task FindById_ShouldReturnCustomer_WhenCustomerExists()
    {
        //Arrange
        var customerId = "ALFKI";
        var companyName = "Test";
        var modelDto = new CustomerDto()
        {
            CustomerID = customerId,
            CompanyName = companyName
        };
        var response = new ResponseService<CustomerDto>(modelDto, null);
        _customerService.Setup(x => x.FindByIdAsync(customerId))!
            .ReturnsAsync(response);

        //Act
        var actionResult = await _sut.FindById(customerId);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = okObjectResult.Value as ResponseService<CustomerDto>;
        Assert.Equal(customerId, value!.Result!.CustomerID);
    }

    [Fact]
    public async Task FindById_ShouldReturnNothing_WhenCustomerNotExists()
    {
        //Arrange
        var response = new ResponseService<CustomerDto>(null);
        _customerService.Setup(x => x.FindByIdAsync("99999"))!
            .ReturnsAsync(response);

        //Act
        var actionResult = await _sut.FindById("99999");

        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var value = notFoundResult.Value as ResponseService<CustomerDto>;
        Assert.Null(value?.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnTrue_WhenRequestIsValid()
    {
        //Arrange
        var request = new CreateCustomerRequest()
        {
            CustomerID = "AlFKI",
            CompanyName = "Test",
            Address = "Lima"
        };
        var response = new ResponseService<bool>(true, null);
        _customerService.Setup(x => x.CreateAsync(request))!
            .ReturnsAsync(response);

        //Act
        var actionResult = await _sut.Create(request);

        //Assert
        var notFoundResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = notFoundResult.Value as ResponseService<bool>;

        Assert.True(value?.SuccessResult);
    }

    /*[Fact]
    public async Task Create_ShouldReturnFalse_WhenExistsConflict()
    {
        // Arrange
        var request = new CreateCustomerRequest()
        {
            CustomerID = "ALFKI"
        };

        _customerService
            .Setup(x => x.CreateAsync(It.IsAny<CreateCustomerRequest>()))
            .ThrowsAsync(new DuplicateKeyException("Customer", request.CustomerID!));

        var httpContext = new DefaultHttpContext()
        {
            Response =
            {
                StatusCode = (int)HttpStatusCode.Conflict
            }
        };

        var responseStream = new MemoryStream();
        httpContext.Response.Body = responseStream;
        var middleware = new ExceptionMiddleware(next: (innerHttpContext) => Task.CompletedTask,null);
        
        // Act
        await middleware.InvokeAsync(httpContext, async () => { await _sut.Create(request); });

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, httpContext.Response.StatusCode);
        
       /* httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var response = JsonConvert.DeserializeObject<ResponseService<bool>>(responseContent);
        
        Assert.NotNull(response);
        Assert.False(response.SuccessResult);#1#
    }*/
}