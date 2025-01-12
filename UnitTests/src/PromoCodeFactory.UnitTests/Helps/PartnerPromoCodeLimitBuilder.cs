using AutoFixture.Kernel;
using PromoCodeFactory.WebHost.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.Helps
{
    public class PartnerPromoCodeLimitBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context) => Create();       
        public object Create() 
        {

            var currentDate = DateTime.UtcNow;
            currentDate.AddDays(new Random().Next(100));
            var result = new SetPartnerPromoCodeLimitRequest()
            {

                EndDate = currentDate,
                Limit = new Random().Next(100)

            };
            return result;
        }
    }
}
