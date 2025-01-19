using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class RoleRepository : InMemoryRepositoryBase<Role>
    {
        public RoleRepository(IDictionary<Guid, Role> data) : base(data)
        {
        }
    }
}
