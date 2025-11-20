using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Abstractions;

public interface IPromoCodeService
{
    Task IncrementAppliedPromoCodesCountAsync(Guid partnerId);
}
