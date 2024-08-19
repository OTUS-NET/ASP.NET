using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly PartnersController _controller;
        private readonly Mock<IRepository<Partner>> _repository;
        //TODO: Add Unit Tests
        
        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _repository = new Mock<IRepository<Partner>>();
            _controller = new PartnersController(_repository.Object);
        }

        /// <summary>
        /// Фабричный метод
        /// </summary>
        /// <returns></returns>
        private SetPartnerPromoCodeLimitRequest SetPartnerPromoCodeLimitRequestFactory()
        {
            return new SetPartnerPromoCodeLimitRequest();
        }

        //Если партнер не найден, то также нужно выдать ошибку 404;
        public void SetPartnerPromoCodeLimitAsync_PartnerNotExist_ReturnsNotFound404()
        {

        }

        //Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        public void SetPartnerPromoCodeLimitAsync_IsActiveFalse_Returns400()
        {

        }


        //Если партнеру выставляется лимит, то мы должны обнулить количество промокодов,
        //которые партнер выдал NumberIssuedPromoCodes,
        //если лимит закончился, то количество не обнуляется;
        public void SetPartnerPromoCodeLimitAsync_SetLimit_NumberIssuedPromoCodesZero()
        {

        }


        //При установке лимита нужно отключить предыдущий лимит;
        public void SetPartnerPromoCodeLimitAsync_SetLimit_CancelPreviousLimit()
        {

        }

        //Лимит должен быть больше 0;
        public void SetPartnerPromoCodeLimitAsync_ZeroLimit_BadRequestLimitMustBeGreaterZero()
        {

        }

        //Нужно убедиться, что сохранили новый лимит в базу данных (это нужно проверить Unit-тестом);
        //Если в текущей реализации найдутся ошибки, то их нужно исправить и желательно написать тест,
        //чтобы они больше не повторялись.
        public void SetPartnerPromoCodeLimitAsync_SetLimit_LimitSavedInDb()
        {

        }
    }
}