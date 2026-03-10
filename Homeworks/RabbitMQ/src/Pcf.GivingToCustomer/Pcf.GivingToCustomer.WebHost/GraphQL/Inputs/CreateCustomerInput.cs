using System;
using System.Collections.Generic;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Inputs
{
    public class CreateCustomerInput
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public List<Guid> PreferenceIds { get; init; } = new();
    }
}

