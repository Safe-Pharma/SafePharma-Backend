namespace SafePharma.Common
{
    public static class PhoneNormalizer
    {
        public static string Normalize(string phone)
        {
            var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            // Egyptian local format: 01XXXXXXXXX (11 digits) → +201XXXXXXXXX
            if (digitsOnly.Length == 11 && digitsOnly.StartsWith("0"))
            {
                return "+2" + digitsOnly;
            }

            // Already has country code without plus: 201XXXXXXXXX (12 digits)
            if (digitsOnly.Length == 12 && digitsOnly.StartsWith("20"))
            {
                return "+" + digitsOnly;
            }

            // Already normalized or some other format — return with + if missing
            return phone.StartsWith("+") ? phone : "+" + digitsOnly;
        }
    }
}