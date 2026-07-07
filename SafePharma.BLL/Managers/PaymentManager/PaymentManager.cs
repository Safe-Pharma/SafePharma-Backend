using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentManager(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        public async Task<GeneralResult<PaymentInstructionsDto>> GetPaymentInstructions(Guid subscriptionId)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetById(subscriptionId);
            if (subscription == null)
                return GeneralResult<PaymentInstructionsDto>.NotFound("Subscription not found.");

            if (subscription.Status != SubscriptionStatus.AwaitingPayment)
                return GeneralResult<PaymentInstructionsDto>.FailResult(
                    $"Payment instructions aren't available while the subscription status is {subscription.Status}.");

            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByTier(subscription.PlanTier);
            if (plan == null)
                return GeneralResult<PaymentInstructionsDto>.FailResult("This plan is no longer configured. Please contact support.");

            var amount = subscription.BillingCycle == "yearly" ? plan.YearlyPrice : plan.MonthlyPrice;
            var methods = await _unitOfWork.PaymentMethodRepository.GetActiveOrdered();

            var dto = new PaymentInstructionsDto
            {
                SubscriptionId = subscription.Id,
                ReferenceCode = subscription.ReferenceCode,
                PlanTier = subscription.PlanTier,
                BillingCycle = subscription.BillingCycle,
                AmountDue = amount,
                Currency = plan.Currency,
                PaymentMethods = methods.Select(m => new PaymentMethodReadDto
                {
                    Id = m.Id,
                    MethodName = m.MethodName,
                    IsActive = m.IsActive,
                    SortOrder = m.SortOrder,
                    Fields = System.Text.Json.JsonSerializer.Deserialize<List<PaymentMethodFieldDto>>(m.FieldsJson) ?? new()
                }).ToList()
            };

            return GeneralResult<PaymentInstructionsDto>.SuccessResult(dto);
        }

        public async Task<GeneralResult<PaymentVerificationReadDto>> SubmitPaymentProof(Guid subscriptionId, SubmitPaymentProofDto dto)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetById(subscriptionId);
            if (subscription == null)
                return GeneralResult<PaymentVerificationReadDto>.NotFound("Subscription not found.");

            if (subscription.Status != SubscriptionStatus.AwaitingPayment)
                return GeneralResult<PaymentVerificationReadDto>.FailResult(
                    $"Cannot submit payment proof while subscription status is {subscription.Status}.");

            if (await _unitOfWork.PaymentVerificationRepository.HasPendingForSubscription(subscriptionId))
                return GeneralResult<PaymentVerificationReadDto>.FailResult(
                    "A payment proof is already pending review for this subscription.");

            if (dto.Receipt == null || dto.Receipt.Length == 0)
                return GeneralResult<PaymentVerificationReadDto>.FailResult("A receipt image is required.");

            var receiptUrl = await _cloudinaryService.UploadImageAsync(dto.Receipt);
            if (string.IsNullOrWhiteSpace(receiptUrl))
                return GeneralResult<PaymentVerificationReadDto>.FailResult("Receipt upload failed. Please attach a valid image.");

            var verification = new PaymentVerification
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                PaymentMethod = dto.PaymentMethod,
                TransactionReference = dto.TransactionReference,
                PaymentDate = dto.PaymentDate,
                PaidAmount = dto.PaidAmount,
                ReceiptUrl = receiptUrl,
                Status = PaymentVerificationStatus.Pending
            };

            _unitOfWork.PaymentVerificationRepository.Add(verification);
            await _unitOfWork.SaveAsync();

            var readDto = new PaymentVerificationReadDto
            {
                Id = verification.Id,
                SubscriptionId = subscription.Id,
                PlanTier = subscription.PlanTier,
                BillingCycle = subscription.BillingCycle,
                PaymentMethod = verification.PaymentMethod,
                TransactionReference = verification.TransactionReference,
                PaymentDate = verification.PaymentDate,
                PaidAmount = verification.PaidAmount,
                ReceiptUrl = verification.ReceiptUrl,
                Status = verification.Status.ToString(),
                CreatedAt = verification.CreatedAt
            };

            return GeneralResult<PaymentVerificationReadDto>.SuccessResult(readDto, "Payment proof submitted. Awaiting review.");
        }

        public async Task<IEnumerable<PaymentVerificationReadDto>> GetPendingVerifications()
        {
            var verifications = await _unitOfWork.PaymentVerificationRepository.GetPendingWithSubscription();
            return verifications.Select(ToReadDto);
        }

        public async Task<GeneralResult> ApprovePayment(Guid verificationId, Guid reviewedByUserId)
        {
            var verification = await _unitOfWork.PaymentVerificationRepository.GetByIdWithSubscription(verificationId);
            if (verification == null)
                return GeneralResult.NotFound("Payment verification not found.");

            if (verification.Status != PaymentVerificationStatus.Pending)
                return GeneralResult.FailResult("This payment verification has already been reviewed.");

            var subscription = verification.Subscription; // tracked, includes Pharmacy
            var primaryContact = await _unitOfWork.PrimaryContactRepository.GetByPharmacyId(subscription.Pharmacy.Id);
            if (primaryContact == null)
                return GeneralResult.FailResult("Primary contact not found for this pharmacy.");

            verification.Status = PaymentVerificationStatus.Approved;
            verification.ReviewedBy = reviewedByUserId;
            verification.ReviewedAt = DateTime.UtcNow;

            subscription.Status = SubscriptionStatus.Active;
            subscription.ApprovedAt = DateTime.UtcNow;
            subscription.ApprovedBy = reviewedByUserId;

            var existingUser = await _userManager.FindByEmailAsync(primaryContact.Email);
            if (existingUser == null)
            {
                var nameParts = primaryContact.FullName.Split(' ', 2);
                var newUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = primaryContact.Email,
                    Email = primaryContact.Email,
                    EmailConfirmed = true,
                    FirstName = nameParts.ElementAtOrDefault(0) ?? primaryContact.FullName,
                    LastName = nameParts.ElementAtOrDefault(1) ?? string.Empty,
                    PharmacyId = subscription.Pharmacy.Id,
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                    return GeneralResult.FailResult(
                        $"Failed to create user account: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

                // Reuse the hash created at registration time — PasswordHasher<T>'s
                // algorithm doesn't depend on T, so this hash is valid for ApplicationUser too.
                newUser.PasswordHash = primaryContact.PasswordHash;
                await _userManager.UpdateAsync(newUser);
                await _userManager.AddToRoleAsync(newUser, "admin");
            }

            primaryContact.IsApproved = true;
            _unitOfWork.PrimaryContactRepository.Update(primaryContact);

            await _unitOfWork.SaveAsync();

            // TODO: plug in a real email/notification service here to confirm activation.

            return GeneralResult.SuccessResult("Payment approved. Subscription is now active.");
        }

        public async Task<GeneralResult> RejectPayment(Guid verificationId, Guid reviewedByUserId, string reason)
        {
            var verification = await _unitOfWork.PaymentVerificationRepository.GetByIdWithSubscription(verificationId);
            if (verification == null)
                return GeneralResult.NotFound("Payment verification not found.");

            if (verification.Status != PaymentVerificationStatus.Pending)
                return GeneralResult.FailResult("This payment verification has already been reviewed.");

            verification.Status = PaymentVerificationStatus.Rejected;
            verification.RejectionReason = reason;
            verification.ReviewedBy = reviewedByUserId;
            verification.ReviewedAt = DateTime.UtcNow;
            // Subscription stays AwaitingPayment so the user can resubmit.

            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult("Payment rejected. The user can submit a new payment proof.");
        }
        private static PaymentVerificationReadDto ToReadDto(PaymentVerification v) => new()
        {
            Id = v.Id,
            SubscriptionId = v.SubscriptionId,
            PharmacyName = v.Subscription?.Pharmacy?.Name,
            PlanTier = v.Subscription?.PlanTier,
            BillingCycle = v.Subscription?.BillingCycle,
            PaymentMethod = v.PaymentMethod,
            TransactionReference = v.TransactionReference,
            PaymentDate = v.PaymentDate,
            PaidAmount = v.PaidAmount,
            ReceiptUrl = v.ReceiptUrl,
            Status = v.Status.ToString(),
            RejectionReason = v.RejectionReason,
            CreatedAt = v.CreatedAt,
            ReviewedAt = v.ReviewedAt
        };
        public async Task<GeneralResult<PaymentVerificationReadDto>> GetLatestVerificationStatus(Guid subscriptionId)
        {
            var verification = await _unitOfWork.PaymentVerificationRepository.GetLatestForSubscription(subscriptionId);
            if (verification == null)
                return GeneralResult<PaymentVerificationReadDto>.NotFound("No payment proof has been submitted for this subscription yet.");

            return GeneralResult<PaymentVerificationReadDto>.SuccessResult(ToReadDto(verification));
        }

        public async Task<IEnumerable<PaymentVerificationReadDto>> GetAllVerifications()
        {
            var verifications = await _unitOfWork.PaymentVerificationRepository.GetAllWithSubscription();
            return verifications.Select(ToReadDto);
        }
    }
}