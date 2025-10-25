using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.GrpcHost.Protos;

namespace Pcf.GivingToCustomer.GrpcHost.Services
{
    public class CustomerGrpcService(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository) : CustomerGrpc.CustomerGrpcBase
    {
        private readonly IRepository<Customer> _customerRepository = customerRepository;
        private readonly IRepository<Preference> _preferenceRepository = preferenceRepository;

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<CustomerShortReplyList> GetCustomers(
            Empty request,
            ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();

            return new CustomerShortReplyList
            {
                Customers =
                {
                    customers.Select(x => new CustomerShortReply
                    {
                        Id = x.Id.ToString(),
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    })
                }
            };
        }

        /// <summary>
        /// Получить клиента по ID 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<CustomerReply> GetCustomer(
            CustomerIdRequest request,
            ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            return MapToCustomerReply(customer);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<CustomerReply> CreateCustomer(
            CreateCustomerRequest request,
            ServerCallContext context)
        {
            var preferences = await _preferenceRepository.GetRangeByIdsAsync([.. request.PreferenceIds.Select(id => Guid.Parse(id))]);
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = [.. preferences
                    .Select(p => new CustomerPreference
                    {
                        PreferenceId = p.Id,
                        Preference = p
                    })]
            };

            await _customerRepository.AddAsync(customer);
            return MapToCustomerReply(customer);
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<Empty> EditCustomer(
            EditCustomerRequest request,
            ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            var preferences = await _preferenceRepository.GetRangeByIdsAsync([.. request.PreferenceIds.Select(id => Guid.Parse(id))]);
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Preferences = [.. preferences
                    .Select(p => new CustomerPreference
                    {
                        CustomerId = customer.Id,
                        PreferenceId = p.Id,
                        Preference = p
                    })];
            await _customerRepository.UpdateAsync(customer);
            return new Empty();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<Empty> DeleteCustomer(
            CustomerIdRequest request,
            ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            await _customerRepository.DeleteAsync(customer);
            return new Empty();
        }

        private static CustomerReply MapToCustomerReply(Customer customer)
        {
            return new CustomerReply
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = { customer.Preferences.Select(p => new PreferenceReply
                {
                    Id = p.PreferenceId.ToString(),
                    Name = p.Preference.Name
                }) },

                PromoCodes = { customer.PromoCodes.Select(pc => new PromoCodeShortReply
                {
                    Id = pc.PromoCode.Id.ToString(),
                    Code = pc.PromoCode.Code,
                    BeginDate = pc.PromoCode.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = pc.PromoCode.EndDate.ToString("yyyy-MM-dd"),
                    PartnerId = pc.PromoCode.PartnerId.ToString(),
                    ServiceInfo = pc.PromoCode.ServiceInfo
                }) }
            };
        }
    }
}
