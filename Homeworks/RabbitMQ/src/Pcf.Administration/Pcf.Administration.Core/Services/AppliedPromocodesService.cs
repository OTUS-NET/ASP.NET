using System;
using System.Threading.Tasks;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.Core.Services
{
    public class AppliedPromocodesService : IAppliedPromocodesService
    {
        private readonly IRepository<Employee> _employeeRepository;

        public AppliedPromocodesService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> IncrementAppliedPromocodesAsync(Guid employeeId, int incrementBy = 1)
        {
            if (incrementBy <= 0)
                throw new ArgumentOutOfRangeException(nameof(incrementBy), incrementBy, "incrementBy must be positive");

            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return false;

            employee.AppliedPromocodesCount += incrementBy;

            await _employeeRepository.UpdateAsync(employee);

            return true;
        }
    }
}

