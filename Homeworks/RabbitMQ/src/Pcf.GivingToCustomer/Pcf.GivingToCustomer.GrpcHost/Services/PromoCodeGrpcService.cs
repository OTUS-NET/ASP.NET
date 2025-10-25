using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.GrpcHost.Protos;
using System.Globalization;

namespace Pcf.GivingToCustomer.GrpcHost.Services
{
    public class PromoCodeGrpcService(IRepository<PromoCode> promoCodeRepository,
        IRepository<Preference> preferenceRepository,
        IRepository<Customer> customerRepository) : PromoCodeGrpc.PromoCodeGrpcBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository = promoCodeRepository;
        private readonly IRepository<Preference> _preferenceRepository = preferenceRepository;
        private readonly IRepository<Customer> _customerRepository = customerRepository;

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<PromoCodeShortReplyList> GetPromoCodes(
             Empty request,
             ServerCallContext context)
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            return new PromoCodeShortReplyList
            {
                PromoCodes =
                {
                    promoCodes.Select(p => new PromoCodeShortReply
                    {
                        Id = p.Id.ToString(),
                        Code = p.Code,
                        ServiceInfo = p.ServiceInfo,
                        BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                        EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                        PartnerId = p.PartnerId.ToString()
                    })
                }
            };
        }

        /// <summary>
        /// Получить промокод по ID
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<PromoCodeReply> GetPromoCode(
            PromoCodeIdRequest request,
            ServerCallContext context)
        {
            var promoCode = await _promoCodeRepository
                .GetByIdAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "PromoCode not found"));
            return MapToPromoCodeReply(promoCode);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<PromoCodeReply> GivePromoCodeToCustomersWithPreference(
            GivePromoCodeRequest request,
            ServerCallContext context)
        {
            //Получаем предпочтение по Id
            var preference = await _preferenceRepository
                .GetByIdAsync(Guid.Parse(request.PreferenceId)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Preference not found"));

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customerRepository
                .GetWhere(c => c.Preferences.Any(cp => cp.PreferenceId == preference.Id));

            // Создание промокода
            var promoCode = new PromoCode
            {
                Id = string.IsNullOrEmpty(request.PromoCodeId) ? Guid.NewGuid() : Guid.Parse(request.PromoCodeId),
                PartnerId = Guid.Parse(request.PartnerId),
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                BeginDate = DateTime.SpecifyKind(
                DateTime.ParseExact(request.BeginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(
                DateTime.ParseExact(request.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeKind.Utc),
                Preference = preference,
                PreferenceId = preference.Id,
                Customers = []
            };

            foreach (var customer in customers)
            {
                promoCode.Customers.Add(new PromoCodeCustomer()
                {
                    CustomerId = customer.Id,
                    Customer = customer,
                    PromoCodeId = promoCode.Id,
                    PromoCode = promoCode
                });
            }
            await _promoCodeRepository.AddAsync(promoCode);
            return MapToPromoCodeReply(promoCode);
        }

        private static PromoCodeReply MapToPromoCodeReply(PromoCode promoCode)
        {
            return new PromoCodeReply
            {
                Id = promoCode.Id.ToString(),
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = promoCode.PartnerId.ToString(),
                Preference = new PreferenceReply
                {
                    Id = promoCode.PreferenceId.ToString(),
                    Name = promoCode.Preference.Name
                },
                Customers =
                {
                    promoCode.Customers.Select(pc => new CustomerShortReply
                    {
                        Id = pc.Customer.Id.ToString(),
                        FirstName = pc.Customer.FirstName,
                        LastName = pc.Customer.LastName,
                        Email = pc.Customer.Email
                    })
                }
            };
        }
    }
}
