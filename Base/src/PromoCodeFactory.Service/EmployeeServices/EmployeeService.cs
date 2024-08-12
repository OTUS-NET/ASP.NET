using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.Employers.ViewModel;
using PromoCodeFactory.Service.Exceptions;
using PromoCodeFactory.Service.Roles;
using PromoCodeFactory.Service.RoleServices.ViewModel;
using System.Runtime.CompilerServices;

namespace PromoCodeFactory.Service.Employers
{
    public class EmployeeService : IEmployeeService
    {
        protected IRepository<Employee> _employeeRepository;
        protected IRoleService _roleService;

        public EmployeeService(IRepository<Employee> employeeRepository, IRoleService roleService)
        {
            _employeeRepository = employeeRepository;
            _roleService = roleService;
        }

        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        public async Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                throw new NotFoundException("Пользователь не найден");

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        public async Task CreateEmployee(EmployeeCreateRequest newEmployee)
        {
            var roles = await BindRoleEmployee(newEmployee.Roles.Select(t => t).ToList());

            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount,
                Email = newEmployee.Email,
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName,
                Roles = roles
            };

            await _employeeRepository.Create(employee);
        }

        private async Task<List<Role>> BindRoleEmployee(IEnumerable<Guid> roleIds)
        {
            var roles = await _roleService.GetRolesFromIdsAsync(roleIds);
            return roles.ToList();
        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                throw new NotFoundException("Пользователь не найден");

            await _employeeRepository.Delete(employee);
        }

        public async Task UpdateEmployeeAsync(EmployeeRequest employeeViewModel)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeViewModel.Id);

            if (employee == null)
                throw new NotFoundException("Пользователь не найден");

            var roleIds = employeeViewModel.Roles.Select(t => t.Id).ToList();
            var roles = await _roleService.GetRolesFromIdsAsync(roleIds);

            employee.Email = employeeViewModel.Email;
            employee.FirstName = employeeViewModel.FirstName;
            employee.LastName = employeeViewModel.LastName;
            employee.AppliedPromocodesCount = employeeViewModel.AppliedPromocodesCount;

            employee.Roles = roles.ToList();
        }
    }
}
