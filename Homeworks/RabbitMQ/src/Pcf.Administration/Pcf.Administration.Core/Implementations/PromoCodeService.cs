using System;
using System.Threading.Tasks;
using Pcf.Administration.Core.Abstractions;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.Core.Implementations;

public class PromoCodeService(IRepository<Employee> employeeRepository) : IPromoCodeService
{
    public async Task IncrementAppliedPromoCodesCountAsync(Guid partnerId)
    {
        var employee = await employeeRepository.GetByIdAsync(partnerId);

        if (employee == null)
        {
            return;
        }

        employee.AppliedPromocodesCount++;
        await employeeRepository.UpdateAsync(employee);
    }
}
