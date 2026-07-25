using FluentValidation;
using SafePharma.BLL.DTOs;
using SafePharma.Common;
using SafePharma.DAL;
using System.Security.Claims;

namespace SafePharma.BLL
{
    public class OtpManager : IOtpManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IOtpDeliveryChannel> _channels;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IValidator<RequestOtpDto> _requestOtpValidator;
        private readonly IValidator<VerifyOtpDto> _verifyOtpValidator;

        public OtpManager(
            IUnitOfWork unitOfWork,
            IEnumerable<IOtpDeliveryChannel> channels,
            ITokenGenerator tokenGenerator,
            IValidator<RequestOtpDto> requestOtpValidator,
            IValidator<VerifyOtpDto> verifyOtpValidator)
        {
            _unitOfWork = unitOfWork;
            _channels = channels;
            _tokenGenerator = tokenGenerator;
            _requestOtpValidator = requestOtpValidator;
            _verifyOtpValidator = verifyOtpValidator;
        }

        public async Task<GeneralResult<string>> RequestOtpAsync(RequestOtpDto dto)
        {
            var validationResult = await _requestOtpValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );

                return GeneralResult<string>.FailResult(errors, "Validation failed");
            }

            var normalizedPhone = PhoneNormalizer.Normalize(dto.Phone);
            var customer = await _unitOfWork.CustomerRepository.GetByPhone(normalizedPhone);
            if (customer == null)
            {
                return GeneralResult<string>.FailResult("No account with this phone number.");
            }

            var otp = GenerateOtp(customer.Id);
            _unitOfWork.OtpRepository.Add(otp);
            await _unitOfWork.SaveAsync();

            foreach (var channel in _channels)
            {
                var result = await channel.SendAsync(normalizedPhone, otp.Code);
                if (result.Success)
                {
                    return GeneralResult<string>.SuccessResult(channel.ChannelName, "OTP sent successfully.");
                }
            }

            return GeneralResult<string>.FailResult("Failed to send OTP through any available channel.");
        }

        public async Task<GeneralResult<TokenDto>> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var validationResult = await _verifyOtpValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );

                return GeneralResult<TokenDto>.FailResult(errors, "Validation failed");
            }

            var normalizedPhone = PhoneNormalizer.Normalize(dto.Phone);
            var customer = await _unitOfWork.CustomerRepository.GetByPhone(normalizedPhone);
            if (customer == null)
            {
                return GeneralResult<TokenDto>.FailResult("No account with this phone number.");
            }

            var otp = await _unitOfWork.OtpRepository.GetValidOtp(customer.Id, dto.Code);
            if (otp == null)
            {
                return GeneralResult<TokenDto>.FailResult("Invalid or expired code.");
            }

            otp.IsUsed = false;
            //otp.IsUsed = true;

            await _unitOfWork.SaveAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim("Phone", customer.Phone),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("Name", customer.Name),


            };

            var token = _tokenGenerator.GenerateToken(claims);

            return GeneralResult<TokenDto>.SuccessResult(token, "Verified successfully.");
        }

        private Otp GenerateOtp(Guid customerId)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();

            return new Otp
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Code = code,
                ExpireDateTime = DateTime.UtcNow.AddMinutes(10000),
                //ExpireDateTime = DateTime.UtcNow.AddMinutes(5),

                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}