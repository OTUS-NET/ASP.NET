using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.DataAccess.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContextInitializer : IDataContextInitializer
    {
        private readonly DataContext _dataContext;
        public DataContextInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SeedAsync(CancellationToken ct)
        {
            try
            {
                await _dataContext.Database.EnsureCreatedAsync(ct);

                await _dataContext.Preferences.AddRangeAsync(FakeDataFactory.Preferences, ct);
                await _dataContext.SaveChangesAsync(ct);

                await _dataContext.Roles.AddRangeAsync(FakeDataFactory.Roles, ct);
                await _dataContext.SaveChangesAsync(ct);

                var employees = FakeDataFactory.Employees.ToList();
                foreach (var employee in employees)
                {
                    employee.Role = _dataContext.Roles.First(r => r.Name == employee.Role.Name);
                }
                await _dataContext.Employees.AddRangeAsync(employees, ct);
                await _dataContext.SaveChangesAsync(ct);

                var customers = FakeDataFactory.Customers.ToList();
                foreach (var customer in customers)
                {
                    foreach (var pref in customer.CustomerPreferences)
                    {
                        pref.Preference = _dataContext.Preferences.First(p => p.Name == pref.Preference.Name);
                    }
                }
                await _dataContext.Customers.AddRangeAsync(customers, ct);
                await _dataContext.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Seed()
        {
            try
            {
                _dataContext.Database.EnsureDeleted();
                _dataContext.Database.EnsureCreated();

                _dataContext.Preferences.AddRange(FakeDataFactory.Preferences);
                _dataContext.SaveChanges();

                _dataContext.Roles.AddRange(FakeDataFactory.Roles);
                _dataContext.SaveChanges();

                var employees = FakeDataFactory.Employees.ToList();
                foreach (var employee in employees)
                {
                    employee.Role = _dataContext.Roles.First(r => r.Name == employee.Role.Name);
                }
                _dataContext.Employees.AddRange(employees);
                _dataContext.SaveChanges();

                var customers = FakeDataFactory.Customers.ToList();
                foreach (var customer in customers)
                {
                    foreach (var pref in customer.CustomerPreferences)
                    {
                        pref.Preference = _dataContext.Preferences.First(p => p.Name == pref.Preference.Name);
                    }
                }
                _dataContext.Customers.AddRange(customers);
                _dataContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
