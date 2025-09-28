using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.UnitTests.WebHost.Helpers
{
    public class PartnerBuilder
    {
        private bool _isActive;
        private int _numberIssuedPromoCodes;
        private bool _isSetLimit;
        private Guid _id;

        public PartnerBuilder()
        {

        }
        public PartnerBuilder SetId(Guid id)
        {
            _id = id;
            return this;
        }
        public PartnerBuilder SetActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }
        public PartnerBuilder SetNumberPromocode(int count)
        {
            _numberIssuedPromoCodes = count;
            return this;
        }

        public PartnerBuilder IsSetCancelDate(bool isSetLimit)
        {
            _isSetLimit = isSetLimit;
            return this;
        }

        public Partner Build()
        {
            return new Partner()
            {
                Id = _id,
                Name = Guid.NewGuid().ToString(),
                IsActive = _isActive,
                NumberIssuedPromoCodes = _numberIssuedPromoCodes,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("c9bef066-3c5a-4e5d-9cff-bd54479f075e"),
                        CreateDate = new DateTime(2020,05,3).ToUniversalTime(),
                        EndDate = new DateTime(2020,10,15).ToUniversalTime(),
                        CancelDate = _isSetLimit ? new DateTime(2020,06,16).ToUniversalTime() : null,
                        Limit = 1000
                    },
                 
                }
            };
        }
    }
}
