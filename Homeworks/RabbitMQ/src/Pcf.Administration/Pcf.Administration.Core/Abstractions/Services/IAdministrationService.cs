using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Abstractions.Services
{
    public interface IAdministrationService
    {
        Task<bool> UpdateAppliedPromocodesAsync(Guid id);
    }
}
