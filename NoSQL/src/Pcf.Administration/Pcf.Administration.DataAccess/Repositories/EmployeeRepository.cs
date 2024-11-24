using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Repositories
{
    public class EmployeeRepository
        : EfRepository<Employee>
    {
        private readonly DataContext _dataContext;
        public DbSet<Employee> _dbSet {  get; init; } 

        public EmployeeRepository(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
            _dbSet = _dataContext.Set<Employee>();
        }

        public override async Task<Employee> GetByIdAsync(string id)
        {
            var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            var role = await _dataContext.Roles.FirstOrDefaultAsync(x => x.Id == entity.RoleId);
            entity.Role = role;
            DetectChangesToConsole();
            return entity;
        }

        private void DetectChangesToConsole()
        {
            _dataContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dataContext.ChangeTracker.DebugView.LongView);
        }
    }
}