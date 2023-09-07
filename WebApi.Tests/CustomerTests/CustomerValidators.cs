using Application.Common.Validations;
using Models.Requests;

namespace WebApi.Tests.CustomerTests;

public class CustomerValidators
{
    private readonly CreateCustomerValidator _createCustomerValidator;

    public CustomerValidators()
    {
        _createCustomerValidator = new CreateCustomerValidator();
    }
    
    [Fact]
    public async Task Should_have_error_when_CustomerRequest_is_InValid()
    {
        //Arrange
        var request = new CreateCustomerRequest()
        {
            CustomerID = null,
            CompanyName = null,
            Address = null
        };
       
        //Act
        var result = await _createCustomerValidator.ValidateAsync(request);

        //Assert
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public async Task NotShould_have_error_when_CustomerRequest_is_Valid()
    {
        //Arrange
        var request = new CreateCustomerRequest()
        {
            CustomerID = "ALFKI",
            CompanyName = "Demo",
            Address = "Lima"
        };
       
        //Act
        var result = await _createCustomerValidator.ValidateAsync(request);

        //Assert
        Assert.True(result.IsValid);
    }
}