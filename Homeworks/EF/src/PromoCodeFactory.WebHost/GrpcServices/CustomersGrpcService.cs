using System.Linq;
using Grpc.Core;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Grpc;
using System;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.GrpcServices;

public class CustomersGrpcService : CustomersGrpc.CustomersGrpcBase
{
    private readonly IRepository<Customer> _customerRepository;

    public CustomersGrpcService(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public override async Task<GetCustomersResponse> GetCustomers(
        GetCustomersRequest request,
        ServerCallContext context)
    {
        var customers = await _customerRepository.GetAllAsync();

        var response = new GetCustomersResponse();

        response.Customers.AddRange(customers.Select(customer => new CustomerShortGrpcResponse
        {
            Id = customer.Id.ToString(),
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email
        }));

        return response;
    }

    public override async Task<CustomerGrpcResponse> GetCustomerById(
        GetCustomerByIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var customerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer id"));
        }

        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
        }

        var response = new CustomerGrpcResponse
        {
            Id = customer.Id.ToString(),
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email
        };

        response.Preferences.AddRange(customer.CustomerPreferences.Select(customerPreference =>
            new PreferenceGrpcResponse
            {
                Id = customerPreference.Preference.Id.ToString(),
                Name = customerPreference.Preference.Name
            }));

        response.PromoCodes.AddRange(customer.PromoCodes.Select(promoCode =>
            new PromoCodeGrpcResponse
            {
                Id = promoCode.Id.ToString(),
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = promoCode.PartnerName
            }));

        return response;
    }
}