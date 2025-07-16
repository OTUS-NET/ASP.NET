using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.RabbitMq;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using RabbitMQ.Client.Events;

namespace Pcf.Administration.WebHost.Services.Employees;

public class EmployeesService : RabbitMqConsumer, IEmployeesService
{
    private readonly IRepository<Employee> _employeeRepository;

    public EmployeesService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    protected override async Task OnConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs e)
    {
        var body = e.Body;
        var message = JsonSerializer.Deserialize<AdministrationDto>(Encoding.UTF8.GetString(body.ToArray()));
        
        var employee = await _employeeRepository.GetByIdAsync(message!.PartnerId);

        if (employee == null)
            return;

        employee.AppliedPromocodesCount++;

        await _employeeRepository.UpdateAsync(employee);
    }
}