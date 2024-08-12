using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service;
using PromoCodeFactory.Service.Employers;
using PromoCodeFactory.Service.Employers.ViewModel;
using PromoCodeFactory.Service.Exceptions;
using PromoCodeFactory.Service.RoleServices.ViewModel;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employerService;

        public EmployeesController(IEmployeeService employerService)
        {
            _employerService = employerService;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employerService.GetEmployeesAsync();

            return employees;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            try
            {
                var employee = await _employerService.GetEmployeeByIdAsync(id);

                return employee;
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task CreateEmployee(EmployeeCreateRequest employee)
        {
            await _employerService.CreateEmployee(employee);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                await _employerService.DeleteEmployeeAsync(id);

                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEmployee(EmployeeRequest employee)
        {
            try
            {
                await _employerService.UpdateEmployeeAsync(employee);

                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}