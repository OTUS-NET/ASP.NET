using System;

namespace Pcf.GivingToCustomer.WebHost.Settings.Exceptions
{
    public static class Comment
    {
        public static string FormatBadRequestErrorMessage(Guid id, string nameOfEntity)
            => $"The {nameOfEntity} It does not have preferences: {id}.";
         
    }
}
