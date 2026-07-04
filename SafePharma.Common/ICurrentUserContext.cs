namespace SafePharma.Common
{
    public interface ICurrentUserContext
    {
        Guid UserId { get; }
        Guid PharmacyId { get; }
        IReadOnlyList<string> Roles { get; }
    }
}
