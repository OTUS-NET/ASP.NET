﻿using System;

namespace Pcf.GivingToCustomer.WebHost.Models
{
    public class EmployeeShortResponse
    {
        public Guid Id { get; set; }
        
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}