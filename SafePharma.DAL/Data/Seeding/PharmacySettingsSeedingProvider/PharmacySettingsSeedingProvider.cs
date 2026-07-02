using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class PharmacySettingsSeedingProvider
    {
        public static PharmacySettings GetDefaultPharmacySettings()
        {
            return new PharmacySettings
            {
                Name = "Default Pharmacy",
                LogoUrl = null,
                Street = null,
                City = null,
                Governorate = null,
                Phone = null,
                TaxRegistrationNumber = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = null
            };
        }
    }
}
