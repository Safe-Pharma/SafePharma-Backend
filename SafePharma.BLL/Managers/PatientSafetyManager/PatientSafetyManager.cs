using SafePharma.Common;
using SafePharma.DAL;
using SafePharma.AI.Agent;
using SafePharma.AI.Contracts;

namespace SafePharma.BLL;

public class PatientSafetyManager : IPatientSafetyManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPatientSafetyAgent _agent;

    public PatientSafetyManager(IUnitOfWork unitOfWork, IPatientSafetyAgent agent)
    {
        _unitOfWork = unitOfWork;
        _agent = agent;
    }

    public async Task<GeneralResult<PatientProfileDto>> LoadPatientProfileAsync(Guid customerId)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdWithSafetyProfile(customerId);

        if (customer is null)
        {
            return GeneralResult<PatientProfileDto>.NotFound("Customer not found.");
        }

        var profile = new PatientProfileDto
        {
            PatientRef = customer.Id.ToString(),
            Name = customer.Name,
            Age = customer.DateOfBirth.HasValue
                ? CalculateAge(customer.DateOfBirth.Value)
                : null,

            Gender = null,
            WeightKg = null,
            IsPregnant = false,
            PregnancyTrimester = null,
            IsLactating = false,

            Allergies = customer.CustomerAllergies
                .Select(ca => ca.Allergy.NameEn)
                .ToList(),

            ChronicConditions = customer.CustomerChronicConditions
                .Select(cc => cc.ChronicCondition.NameEn)
                .ToList(),

            OrganImpairments = customer.CustomerOrganFunctions
                .Select(cof => new OrganFunctionDto
                {
                    OrganName = cof.Organ.NameEn,
                    ImpairmentLevel = cof.OrganImpairmentLevel.NameEn
                })
                .ToList(),

            CurrentMedications = customer.MedicineHistory
                .Select(h => new DrugInfoDto
                {
                    ClientRef = null,
                    TradeName = h.TradeName ?? h.Medicine?.TradeNameEn ?? "Unknown",
                    ScientificName = h.ScientificName ?? h.Medicine?.ScientificName ?? "Unknown"
                })
                .ToList()
        };

        return GeneralResult<PatientProfileDto>.SuccessResult(profile);
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    private async Task<GeneralResult<PatientSafetyCheckRequest>> BuildRequestAsync(
    Guid pharmacyId,
    Guid customerId,
    IEnumerable<(Guid PharmacyMedicineId, Guid SaleItemId)> itemsToCheck,
    string language)
    {
        var profileResult = await LoadPatientProfileAsync(customerId);
        if (!profileResult.Success || profileResult.Data is null)
        {
            return GeneralResult<PatientSafetyCheckRequest>.NotFound(profileResult.Message);
        }

        var drugsToCheck = new List<DrugInfoDto>();

        foreach (var item in itemsToCheck)
        {
            var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                .GetByIdAndPharmacy(item.PharmacyMedicineId, pharmacyId);

            if (pharmacyMedicine is null)
            {
                continue;
            }

            drugsToCheck.Add(new DrugInfoDto
            {
                ClientRef = item.SaleItemId.ToString(),
                TradeName = pharmacyMedicine.TradeNameEn,
                ScientificName = pharmacyMedicine.ScientificName,
                Strength = pharmacyMedicine.Strength,
                DosageForm = pharmacyMedicine.DosageForm,
                ActiveIngredient = null,
                AtcClassification = null,
                Route = null
            });
        }

        if (drugsToCheck.Count == 0)
        {
            return GeneralResult<PatientSafetyCheckRequest>.FailResult("No valid medicines found to check.");
        }

        var request = new PatientSafetyCheckRequest
        {
            SaleRef = null,
            Language = language,
            Patients =
            [
                new PatientCheckGroup
            {
                Profile = profileResult.Data,
                DrugsToCheck = drugsToCheck
            }
            ]
        };

        return GeneralResult<PatientSafetyCheckRequest>.SuccessResult(request);
    }

    public async Task<GeneralResult<PatientSafetyCheckResponse>> CheckAsync(
     Guid pharmacyId,
     Guid customerId,
     IEnumerable<(Guid PharmacyMedicineId, Guid SaleItemId)> itemsToCheck,
     string language = "en")
    {
        var requestResult = await BuildRequestAsync(pharmacyId, customerId, itemsToCheck, language);
        if (!requestResult.Success || requestResult.Data is null)
        {
            return requestResult.Message == "No valid medicines found to check."
                ? GeneralResult<PatientSafetyCheckResponse>.FailResult(requestResult.Message)
                : GeneralResult<PatientSafetyCheckResponse>.NotFound(requestResult.Message);
        }

        var checkResponse = await _agent.CheckAsync(requestResult.Data);
        return GeneralResult<PatientSafetyCheckResponse>.SuccessResult(checkResponse);
    }

    public async Task<GeneralResult<PatientSafetyCheckResponse>> CheckAsync(
        Guid pharmacyId,
        IEnumerable<PatientCheckRequestGroup> patients,
        string language = "en")
    {
        var groups = new List<PatientCheckGroup>();
        var failures = new List<PatientSafetyResult>();

        foreach (var patient in patients)
        {
            var profileResult = await LoadPatientProfileAsync(patient.CustomerId);
            if (!profileResult.Success || profileResult.Data is null)
            {
                failures.Add(new PatientSafetyResult
                {
                    PatientRef = patient.CustomerId.ToString(),
                    CheckSucceeded = false,
                    FailureReason = profileResult.Message
                });
                continue;
            }

            var drugsToCheck = new List<DrugInfoDto>();
            foreach (var item in patient.Items)
            {
                var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                    .GetByIdAndPharmacy(item.PharmacyMedicineId, pharmacyId);

                if (pharmacyMedicine is null)
                {
                    continue;
                }

                drugsToCheck.Add(new DrugInfoDto
                {
                    ClientRef = item.SaleItemId.ToString(),
                    TradeName = pharmacyMedicine.TradeNameEn,
                    ScientificName = pharmacyMedicine.ScientificName,
                    Strength = pharmacyMedicine.Strength,
                    DosageForm = pharmacyMedicine.DosageForm,
                    ActiveIngredient = null,
                    AtcClassification = null,
                    Route = null
                });
            }

            if (drugsToCheck.Count == 0)
            {
                failures.Add(new PatientSafetyResult
                {
                    PatientRef = patient.CustomerId.ToString(),
                    CheckSucceeded = false,
                    FailureReason = "No valid medicines found to check."
                });
                continue;
            }

            groups.Add(new PatientCheckGroup
            {
                Profile = profileResult.Data,
                DrugsToCheck = drugsToCheck
            });
        }

        // Every patient failed to build a valid group (not found / no valid medicines) —
        // report the failures without ever calling the agent.
        if (groups.Count == 0)
        {
            return GeneralResult<PatientSafetyCheckResponse>.SuccessResult(
                new PatientSafetyCheckResponse { Results = failures });
        }

        var request = new PatientSafetyCheckRequest
        {
            SaleRef = null,
            Language = language,
            Patients = groups
        };

        var checkResponse = await _agent.CheckAsync(request);

        // A patient who failed to even build (see above) still needs to show up in the
        // response — "check all" must never silently drop someone the agent never saw.
        var mergedResults = checkResponse.Results.Concat(failures).ToList();
        return GeneralResult<PatientSafetyCheckResponse>.SuccessResult(
            new PatientSafetyCheckResponse { Results = mergedResults });
    }

    public async IAsyncEnumerable<PatientSafetyStreamEvent> CheckStreamAsync(
        Guid pharmacyId,
        Guid customerId,
        IEnumerable<(Guid PharmacyMedicineId, Guid SaleItemId)> itemsToCheck,
        string language = "en",
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestResult = await BuildRequestAsync(pharmacyId, customerId, itemsToCheck, language);

        if (!requestResult.Success || requestResult.Data is null)
        {
            yield return new PatientSafetyStreamEvent
            {
                PatientRef = customerId.ToString(),
                Type = PatientSafetyStreamEventType.Result,
                Result = new PatientSafetyResult
                {
                    PatientRef = customerId.ToString(),
                    CheckSucceeded = false,
                    FailureReason = requestResult.Message
                }
            };
            yield break;
        }

        await foreach (var evt in _agent.CheckStreamAsync(requestResult.Data, cancellationToken))
        {
            yield return evt;
        }
    }
}