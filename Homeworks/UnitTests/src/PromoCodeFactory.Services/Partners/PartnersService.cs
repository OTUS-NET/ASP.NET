using AutoMapper;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Date.Abstractions;
using PromoCodeFactory.Services.Partners.Abstractions;
using PromoCodeFactory.Services.Partners.Dto;
using PromoCodeFactory.Services.Partners.Exceptions;

namespace PromoCodeFactory.Services.Partners;

public class PartnersService : IPartnersService
{
    private readonly IRepository<Partner> _partnersRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public PartnersService(IRepository<Partner> partnersRepository, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _partnersRepository = partnersRepository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<List<PartnerDto>> GetPartnersAsync()
    {
        var partners = await _partnersRepository.GetAllAsync();
        var partnerDtos = _mapper.Map<List<PartnerDto>>(partners);
        return partnerDtos.ToList();
    }

    public async Task<PartnerPromoCodeLimit> GetPartnerLimitAsync(Guid id, Guid limitId)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);

        if (partner == null)
            throw new PartnerNotFoundException();
            
        var limit = partner.PartnerLimits
            .FirstOrDefault(x => x.Id == limitId);
            
        return limit;
    }

    public async Task<PartnerPromoCodeLimit> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitDto dto)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);

        if (partner == null)
            throw new PartnerNotFoundException();
            
        if (!partner.IsActive)
            throw new PartnerIsNotActiveException();
            
        //Установка лимита партнеру
        var activeLimit = partner.PartnerLimits.FirstOrDefault(x => 
            !x.CancelDate.HasValue);
            
        if (activeLimit != null)
        {
            //Если партнеру выставляется лимит, то мы 
            //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
            //то количество не обнуляется
            partner.NumberIssuedPromoCodes = 0;
                
            //При установке лимита нужно отключить предыдущий лимит
            activeLimit.CancelDate = _dateTimeProvider.CurrentDateTime;
        }

        if (dto.Limit <= 0)
            throw new IncorrectLimitException();
            
        var newLimit = new PartnerPromoCodeLimit()
        {
            Limit = dto.Limit,
            Partner = partner,
            PartnerId = partner.Id,
            CreateDate = _dateTimeProvider.CurrentDateTime,
            EndDate = dto.EndDate
        };
            
        partner.PartnerLimits.Add(newLimit);

        await _partnersRepository.UpdateAsync(partner);

        return newLimit;
    }

    public async Task CancelPartnerPromoCodeLimitAsync(Guid id)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);
            
        if (partner == null)
            throw new PartnerNotFoundException();
            
        //Если партнер заблокирован, то нужно выдать исключение
        if (!partner.IsActive)
            throw new PartnerIsNotActiveException();
            
        //Отключение лимита
        var activeLimit = partner.PartnerLimits.FirstOrDefault(x => 
            !x.CancelDate.HasValue);
            
        if (activeLimit != null)
        {
            activeLimit.CancelDate = _dateTimeProvider.CurrentDateTime;
        }

        await _partnersRepository.UpdateAsync(partner);
    }
}