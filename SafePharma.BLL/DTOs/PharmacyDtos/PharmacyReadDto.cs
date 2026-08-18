using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL.DTOs.PharmacyDtos
{
    public class PharmacyReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

 
        public string? CommercialRegistration { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string BusinessEmail { get; set; } = string.Empty;

        //public int NumberOfBranches { get; set; }

        //public string PreferredLanguage { get; set; } = string.Empty;

        //public string TimeZone { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public Guid SubscriptionId { get; set; }

        //public DateTime CreatedAt { get; set; }

        //public DateTime? UpdatedAt { get; set; }
    }
}
