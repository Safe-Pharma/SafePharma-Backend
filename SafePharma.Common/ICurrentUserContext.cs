namespace SafePharma.Common
{
    public interface ICurrentUserContext
    {
        Guid Id { get; }

        string Name { get; }

        string Phone { get; }

        Guid PharmacyId { get; }

        bool IsCustomer { get; }

        bool IsStaff { get; }

        IReadOnlyList<string> Roles { get; }
    }
}
