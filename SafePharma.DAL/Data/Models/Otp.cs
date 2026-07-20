using System;

namespace SafePharma.DAL
{
    public class Otp
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public DateTime ExpireDateTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}