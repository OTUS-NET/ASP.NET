using System;
using System.Collections.Generic;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<Preference> Preferences => new List<Preference>()
        {
            new Preference()
            {
                Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                Name = "Театр",
            },
            new Preference()
            {
                Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                Name = "Семья",
            },
            new Preference()
            {
                Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                Name = "Дети",
            }
        };

        public static List<Partner> Partners => new List<Partner>()
        {
            new Partner()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020,07,9),
                        EndDate = new DateTime(2020,10,9),
                        Limit = 100
                    }
                }
            },
            new Partner()
            {
                Id = Guid.Parse("894b6e9b-eb5f-406c-aefa-8ccb35d39319"),
                Name = "Каждому кота",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("c9bef066-3c5a-4e5d-9cff-bd54479f075e"),
                        CreateDate = new DateTime(2020,05,3),
                        EndDate = new DateTime(2020, 10, 15),
                        CancelDate = new DateTime(2020, 06, 16),
                        Limit = 1000
                    },
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("0e94624b-1ff9-430e-ba8d-ef1e3b77f2d5"),
                        CreateDate = new DateTime(2020, 05, 3),
                        EndDate = new DateTime(2020, 10, 15),
                        Limit = 100
                    },
                }
            },
            new Partner()
            {
                Id = Guid.Parse("0da65561-cf56-4942-bff2-22f50cf70d43"),
                Name = "Рыба твоей мечты",
                IsActive = false,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("0691bb24-5fd9-4a52-a11c-34bb8bc9364e"),
                        CreateDate = new DateTime(2020, 07, 3),
                        EndDate = DateTime.Now.AddMonths(1),
                        Limit = 100
                    }
                }
            },
            new Partner()
            {
                Id = Guid.Parse("20d2d612-db93-4ed5-86b1-ff2413bca655"),
                Name = "Промотеатры",
                IsActive = false,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("93f3a79d-e9f9-47e6-98bb-1f618db43230"),
                        CreateDate = new DateTime(2020, 09, 6),
                        EndDate = DateTime.Now.AddMonths(1),
                        Limit = 15
                    }
                }
            }
        };
    }
}