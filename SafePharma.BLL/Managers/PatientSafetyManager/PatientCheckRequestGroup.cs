namespace SafePharma.BLL;

/// <summary>
/// One patient's worth of input for a multi-patient safety check: which customer,
/// and which sale-item/medicine pairs to check for them. Mirrors the shape used by
/// "Check all" on an invoice with items split across different family members.
/// </summary>
public record PatientCheckRequestGroup(
    Guid CustomerId,
    IReadOnlyList<(Guid PharmacyMedicineId, Guid SaleItemId)> Items);
