using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Abstractions.Services
{
    public interface IAppliedPromocodesService
    {
        Task<bool> IncrementAppliedPromocodesAsync(Guid employeeId, int incrementBy = 1);
    }
}

