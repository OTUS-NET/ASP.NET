using System;

namespace PromoCodeFactory.Service.RoleServices.ViewModel
{
    public abstract class RoleItemBase
    {
        public Guid Id { get; set; }
    }

    public class RoleItemRequst : RoleItemBase 
    { 
    }

    public class RoleItemResponse : RoleItemBase
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}