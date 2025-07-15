using System.Text;
using System.Text.Json;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Infrastructure.RabbitMq;
using RabbitMQ.Client.Events;

namespace Pcf.Administration.Services.Employees;

public class EmployeesService : RabbitMqConsumer<Employee>, IEmployeesService
{
    private readonly IRepository<Employee> _employeeRepository;

    public EmployeesService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    protected override async Task OnConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs e)
    {
        var body = e.Body;
        var message = JsonSerializer.Deserialize<EmployeesMessage>(Encoding.UTF8.GetString(body.ToArray()));
        
        var employee = await _employeeRepository.GetByIdAsync(message!.Id);

        if (employee == null)
            return;

        employee.AppliedPromocodesCount++;

        await _employeeRepository.UpdateAsync(employee);
    }
}