using System.Net;
using System.Net.Http.Json;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Tests;

public class EmployeeApiTests(WebHostFixture factory) : IClassFixture<WebHostFixture>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public void FakeData_Contains_Two_Employees()
    {
        Assert.Equal(2, FakeDataFactory.Employees.Count);
    }
    
    [Fact]
    public void FakeData_Contains_Two_Roles()
    {
        Assert.Equal(2, FakeDataFactory.Roles.Count);
    }
    
    [Fact]
    public async Task EmployeeCreation_Succeed()
    {
        var employeeRequest = new EmployeeCreationRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "jd@ya.ru"
        };
        
        var response = await _client.PostAsJsonAsync(factory.ApiRoot + "Employees", employeeRequest);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal(employeeRequest.Email, content.Email);
        Assert.Equal($"{employeeRequest.FirstName} {employeeRequest.LastName}", content.FullName);
    }
    
    [Fact]
    public async Task Employee_Fails_On_Wrong_Body()
    {
        var response = await _client.PostAsJsonAsync(factory.ApiRoot + "Employees", 
            new
            {
                FullName = "John Doe",
                Email = "mymail"
            });
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Employee_Persists_Between_Calls()
    {
        var employeeRequest = new EmployeeCreationRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "jd@ya.ru"
        };
        
        var response = await _client.PostAsJsonAsync($"{factory.ApiRoot}Employees", employeeRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdEmployee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(createdEmployee);
        Assert.NotEqual(Guid.Empty, createdEmployee.Id);
        
        response = await _client.GetAsync($"{factory.ApiRoot}Employees/{createdEmployee.Id:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var fetchedEmployee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(fetchedEmployee);
        Assert.Equal(createdEmployee.Id, fetchedEmployee.Id);
        Assert.Equal(createdEmployee.Email, fetchedEmployee.Email);
        Assert.Equal(createdEmployee.FullName, fetchedEmployee.FullName);
        Assert.Equal(createdEmployee.Roles, fetchedEmployee.Roles);
        Assert.Equal(createdEmployee.AppliedPromocodesCount, fetchedEmployee.AppliedPromocodesCount);
    }
    
    [Fact]
    public async Task Employee_Get_Fails_On_Wrong_Id()
    {
        var response = await _client.GetAsync($"{factory.ApiRoot}Employees/{Guid.NewGuid():D}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<EmployeeResponse> GetFirstEmployee()
    {
        var response = await _client.GetAsync($"{factory.ApiRoot}Employees");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
         
        var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeShortResponse>>();
        Assert.NotNull(employees);
        var employeeShort = employees.FirstOrDefault();
        Assert.NotNull(employeeShort);
        
        var employeeResponse = await _client.GetAsync($"{factory.ApiRoot}Employees/{employeeShort.Id:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var employeeFull = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(employeeFull);
        
        return employeeFull;
    }
    
    [Fact]
    public async Task Employee_Get_Without_Id_Returns_Collection()
    {
        var response = await _client.GetAsync($"{factory.ApiRoot}Employees");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
         
        var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeShortResponse>>();
        Assert.NotNull(employees);
        Assert.NotEmpty(employees);
    }
    
    [Fact]
    public async Task Employee_Get_Employee_On_Right_Id()
    {
        var sourceEmployee = await GetFirstEmployee();
        
        var response = await _client.GetAsync($"{factory.ApiRoot}Employees/{sourceEmployee.Id:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(employee);
        Assert.Equal(sourceEmployee.Id, employee.Id);
        Assert.Equal(sourceEmployee.Email, employee.Email);
        Assert.Equal(sourceEmployee.FullName, employee.FullName);
        Assert.Equal(sourceEmployee.Roles, employee.Roles);
    }
    
    [Fact]
    public async Task Employee_Updates_Existing_Employee()
    {
        var sourceEmployee = await GetFirstEmployee();
        Assert.NotNull(sourceEmployee);

        sourceEmployee.Email += "+";

        var response = await _client.PutAsJsonAsync($"{factory.ApiRoot}Employees/{sourceEmployee.Id:D}", sourceEmployee);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var updatedEmployee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(updatedEmployee);
        Assert.Equal(updatedEmployee.Email, sourceEmployee.Email);
        // Assert.Equal(createdEmployee.Id, fetchedEmployee.Id);
        // Assert.Equal(createdEmployee.Email, fetchedEmployee.Email);
        // Assert.Equal(createdEmployee.FullName, fetchedEmployee.FullName);
        // Assert.Equal(createdEmployee.Roles, fetchedEmployee.Roles);
        // Assert.Equal(createdEmployee.AppliedPromocodesCount, fetchedEmployee.AppliedPromocodesCount);
    }
    
}