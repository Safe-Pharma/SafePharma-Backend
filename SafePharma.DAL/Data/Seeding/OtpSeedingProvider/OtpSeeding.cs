namespace SafePharma.DAL
{
    public static class OtpSeeding
    {
        public static List<Otp> GetOtps(Guid customerId)
        {
            return new List<Otp>
            {
                new Otp
                {
                    Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    CustomerId = customerId,
                    Code = "123456",
                    ExpireDateTime = DateTime.UtcNow.AddYears(1),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow,
                }
            };
        }
    }
}