using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Grpc
{
    public class CustomersGrpcService : Customers.CustomersBase
    {
        private readonly IRepository<Pcf.GivingToCustomer.Core.Domain.Customer> _customerRepository;
        private readonly IRepository<Pcf.GivingToCustomer.Core.Domain.Preference> _preferenceRepository;

        public CustomersGrpcService(
            IRepository<Pcf.GivingToCustomer.Core.Domain.Customer> customerRepository,
            IRepository<Pcf.GivingToCustomer.Core.Domain.Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        public override async Task<ListCustomersResponse> ListCustomers(ListCustomersRequest request, ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();

            var response = new ListCustomersResponse();
            response.Customers.AddRange(customers.Select(c => new CustomerShort
            {
                Id = c.Id.ToString(),
                FirstName = c.FirstName ?? string.Empty,
                LastName = c.LastName ?? string.Empty,
                Email = c.Email ?? string.Empty
            }));

            return response;
        }

        public override async Task<Customer> GetCustomer(GetCustomerRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer id"));

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));

            var response = new Customer
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };

            if (customer.Preferences != null)
            {
                response.Preferences.AddRange(customer.Preferences.Select(p => new Preference
                {
                    Id = p.PreferenceId.ToString(),
                    Name = p.Preference?.Name ?? string.Empty
                }));
            }

            if (customer.PromoCodes != null)
            {
                response.PromoCodes.AddRange(customer.PromoCodes.Select(pc => new PromoCodeShort
                {
                    Id = pc.PromoCodeId.ToString(),
                    Code = pc.PromoCode?.Code ?? string.Empty,
                    BeginDate = pc.PromoCode?.BeginDate.ToString("yyyy-MM-dd") ?? string.Empty,
                    EndDate = pc.PromoCode?.EndDate.ToString("yyyy-MM-dd") ?? string.Empty,
                    PartnerId = pc.PromoCode?.PartnerId.ToString() ?? string.Empty,
                    ServiceInfo = pc.PromoCode?.ServiceInfo ?? string.Empty
                }));
            }

            return response;
        }

        public override async Task<CustomerIdResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
        {
            var preferenceIds = request.PreferenceIds
                .Select(x => Guid.TryParse(x, out var id) ? id : Guid.Empty)
                .Where(x => x != Guid.Empty)
                .ToList();

            var preferences = await _preferenceRepository.GetRangeByIdsAsync(preferenceIds);

            var model = new CreateOrEditCustomerRequest
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PreferenceIds = preferenceIds
            };

            var customer = CustomerMapper.MapFromModel(model, preferences);
            await _customerRepository.AddAsync(customer);

            return new CustomerIdResponse { Id = customer.Id.ToString() };
        }

        public override async Task<Empty> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer id"));

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));

            var preferenceIds = request.PreferenceIds
                .Select(x => Guid.TryParse(x, out var parsed) ? parsed : Guid.Empty)
                .Where(x => x != Guid.Empty)
                .ToList();

            var preferences = await _preferenceRepository.GetRangeByIdsAsync(preferenceIds);

            var model = new CreateOrEditCustomerRequest
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PreferenceIds = preferenceIds
            };

            CustomerMapper.MapFromModel(model, preferences, customer);
            await _customerRepository.UpdateAsync(customer);

            return new Empty();
        }

        public override async Task<Empty> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer id"));

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));

            await _customerRepository.DeleteAsync(customer);
            return new Empty();
        }
    }
}

