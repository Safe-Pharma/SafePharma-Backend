using SafePharma.DAL;

public interface IOtpRepository : IGenircRepository<Otp>
{
    Task<Otp?> GetValidOtp(Guid customerId, string code);
}