using PromoCodeFactory.Core.Abstractions.Entities;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Helpers
{
    public class EmployeeHepler : IEntityHepler<Employee>
    {
        public Employee UpdateEntity(Employee actual, Employee excepted)
        {
            if (actual != null)
            {
                actual.FirstName = excepted.FirstName;
                actual.LastName = excepted.LastName;
                actual.Email = excepted.Email;
                actual.Roles = new(excepted.Roles);
                actual.AppliedPromocodesCount = excepted.AppliedPromocodesCount;
            }

            return actual;
        }
    }
}
