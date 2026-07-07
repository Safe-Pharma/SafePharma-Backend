using Microsoft.AspNetCore.Identity;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<PrimaryContact> _passwordHasher;

        public SubscriptionManager(IUnitOfWork unitOfWork, IPasswordHasher<PrimaryContact> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<GeneralResult<SubscriptionReadDto>> CreateSubscription(CreateSubscriptionDto dto)
        {
            if (await _unitOfWork.PharmacyRepository.BusinessEmailExists(dto.Pharmacy.BusinessEmail))
            {
                var errors = new Dictionary<string, List<Error>>
                {
                    ["Pharmacy.BusinessEmail"] = new List<Error>
                    {
                        new Error { ErrorCode = "DUPLICATE_BUSINESS_EMAIL", ErrorMessage = "A pharmacy is already registered with this business email." }
                    }
                };
                return GeneralResult<SubscriptionReadDto>.FailResult(errors);
            }

            if (await _unitOfWork.PrimaryContactRepository.EmailExists(dto.PrimaryContact.Email))
            {
                var errors = new Dictionary<string, List<Error>>
                {
                    ["PrimaryContact.Email"] = new List<Error>
                    {
                        new Error { ErrorCode = "DUPLICATE_CONTACT_EMAIL", ErrorMessage = "This email is already registered as a primary contact." }
                    }
                };
                return GeneralResult<SubscriptionReadDto>.FailResult(errors);
            }
            if (!string.IsNullOrWhiteSpace(dto.Pharmacy.TaxNumber) &&
             await _unitOfWork.PharmacyRepository.TaxNumberExists(dto.Pharmacy.TaxNumber))
            {
                var errors = new Dictionary<string, List<Error>>
                {
                    ["Pharmacy.TaxNumber"] = new List<Error>
                    {
                        new Error { ErrorCode = "DUPLICATE_TAX_NUMBER", ErrorMessage = "This tax number is already registered to another pharmacy." }
                    }
                };
                return GeneralResult<SubscriptionReadDto>.FailResult(errors);
            }

            if (!string.IsNullOrWhiteSpace(dto.Pharmacy.CommercialRegistration) &&
                await _unitOfWork.PharmacyRepository.CommercialRegistrationExists(dto.Pharmacy.CommercialRegistration))
            {
                var errors = new Dictionary<string, List<Error>>
                {
                    ["Pharmacy.CommercialRegistration"] = new List<Error>
                    {
                        new Error { ErrorCode = "DUPLICATE_COMMERCIAL_REGISTRATION", ErrorMessage = "This commercial registration is already registered to another pharmacy." }
                    }
                };
                return GeneralResult<SubscriptionReadDto>.FailResult(errors);
            }

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                PlanTier = dto.PlanTier,
                BillingCycle = dto.BillingCycle,
                Status = SubscriptionStatus.AwaitingPayment,   // was: SubscriptionStatus.PendingReview
                CreatedAt = DateTime.UtcNow
            };

            var pharmacy = new Pharmacy
            {
                Id = Guid.NewGuid(),
                Name = dto.Pharmacy.Name,
                LogoUrl = dto.Pharmacy.LogoUrl,
                TaxNumber = dto.Pharmacy.TaxNumber,
                CommercialRegistration = dto.Pharmacy.CommercialRegistration,
                Address = dto.Pharmacy.Address,
                Country = dto.Pharmacy.Country,
                City = dto.Pharmacy.City,
                Phone = dto.Pharmacy.Phone,
                BusinessEmail = dto.Pharmacy.BusinessEmail,
                NumberOfBranches = dto.Pharmacy.NumberOfBranches,
                PreferredLanguage = dto.Pharmacy.PreferredLanguage,
                TimeZone = dto.Pharmacy.TimeZone,
                CreatedAt = DateTime.UtcNow,
                SubscriptionId = subscription.Id
            };

            var primaryContact = new PrimaryContact
            {
                Id = Guid.NewGuid(),
                FullName = dto.PrimaryContact.FullName,
                Mobile = dto.PrimaryContact.Mobile,
                Email = dto.PrimaryContact.Email,
                IsApproved = false,
                PharmacyId = pharmacy.Id
            };
            primaryContact.PasswordHash = _passwordHasher.HashPassword(primaryContact, dto.PrimaryContact.Password);

            _unitOfWork.SubscriptionRepository.Add(subscription);
            _unitOfWork.PharmacyRepository.Add(pharmacy);
            _unitOfWork.PrimaryContactRepository.Add(primaryContact);

            await _unitOfWork.SaveAsync();

            var readDto = new SubscriptionReadDto
            {
                Id = subscription.Id,
                PlanTier = subscription.PlanTier,
                BillingCycle = subscription.BillingCycle,
                Status = subscription.Status.ToString(),
                CreatedAt = subscription.CreatedAt,
                PharmacyId = pharmacy.Id,
                PharmacyName = pharmacy.Name,
                PrimaryContactEmail = primaryContact.Email
            };

            return GeneralResult<SubscriptionReadDto>.SuccessResult(readDto, "Subscription submitted successfully. Awaiting review.");
        }

        public async Task<IEnumerable<SubscriptionReadDto>> GetAllSubscriptions()
        {
            var subscriptions = await _unitOfWork.SubscriptionRepository.GetAllWithPharmacy();
            return subscriptions.Select(ToReadDto);
        }

        public async Task<GeneralResult<SubscriptionReadDto>> GetSubscriptionById(Guid id)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdWithPharmacy(id);
            if (subscription == null)
                return GeneralResult<SubscriptionReadDto>.NotFound("Subscription not found.");

            return GeneralResult<SubscriptionReadDto>.SuccessResult(ToReadDto(subscription));
        }

        public async Task<GeneralResult<SubscriptionReadDto>> UpdateSubscription(Guid id, UpdateSubscriptionDto dto)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdWithPharmacyTracked(id);
            if (subscription == null)
                return GeneralResult<SubscriptionReadDto>.NotFound("Subscription not found.");

            if (subscription.Status == SubscriptionStatus.Cancelled)
                return GeneralResult<SubscriptionReadDto>.FailResult("Cannot modify a cancelled subscription.");

            subscription.PlanTier = dto.PlanTier;
            subscription.BillingCycle = dto.BillingCycle;

            await _unitOfWork.SaveAsync();

            return GeneralResult<SubscriptionReadDto>.SuccessResult(ToReadDto(subscription), "Subscription updated.");
        }

        public async Task<GeneralResult> CancelSubscription(Guid id, Guid adminId)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdWithPharmacyTracked(id);
            if (subscription == null)
                return GeneralResult.NotFound("Subscription not found.");

            if (subscription.Status == SubscriptionStatus.Cancelled)
                return GeneralResult.FailResult("Subscription is already cancelled.");

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.ApprovedAt = DateTime.UtcNow;   // reusing as "last admin action" timestamp
            subscription.ApprovedBy = adminId;

            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult("Subscription cancelled.");
        }

        private static SubscriptionReadDto ToReadDto(Subscription s) => new()
        {
            Id = s.Id,
            PlanTier = s.PlanTier,
            BillingCycle = s.BillingCycle,
            Status = s.Status.ToString(),
            CreatedAt = s.CreatedAt,
            PharmacyId = s.Pharmacy?.Id ?? Guid.Empty,
            PharmacyName = s.Pharmacy?.Name,
            PrimaryContactEmail = null // not loaded here — fetch separately if you need it in the list view
        };
    }
}