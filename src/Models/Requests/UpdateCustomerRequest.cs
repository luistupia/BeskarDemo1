namespace Models.Requests;

public class UpdateCustomerRequest
{
    public string? CompanyName {get;set;}
    public string? ContactName {get;set;}
    public string? ContactTitle {get;set;}
    public string? Address {get;set;}
    public string? City {get;set;}
    public string? Region {get;set;}
}
