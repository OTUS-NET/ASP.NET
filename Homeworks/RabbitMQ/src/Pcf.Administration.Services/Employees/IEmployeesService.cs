using Pcf.Infrastructure.RabbitMq;

namespace Pcf.Administration.Services.Employees;

public interface IEmployeesService : IRabbitMqConsumer<EmployeesMessage>
{
    
}