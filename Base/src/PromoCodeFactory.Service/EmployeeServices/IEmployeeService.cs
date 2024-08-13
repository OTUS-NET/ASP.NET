using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.Employers.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Service.Employers
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Возвращает список Сотрудников
        /// </summary>
        /// <returns></returns>
        Task<List<EmployeeShortResponse>> GetEmployeesAsync();

        /// <summary>
        /// Возвращает сотрудника по его Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Сотрудник или исключение в случае если сотрудник не найден</returns>
        Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id);

        /// <summary>
        /// Добавление нового сотрудника
        /// </summary>
        /// <param name="EmployeeCreateRequest"></param>
        /// <returns></returns>
        Task CreateEmployee(EmployeeCreateRequest EmployeeCreateRequest);

        /// <summary>
        /// Удаление сотрудника по его Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>В случае если сотрудник не найден то генерируется исключение</returns>
        Task DeleteEmployeeAsync(Guid id);

        /// <summary>
        /// Обновление данных сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>В случае если сотрудник не найден то генерируется исключение</returns>
        Task UpdateEmployeeAsync(EmployeeRequest employee);
    }
}
