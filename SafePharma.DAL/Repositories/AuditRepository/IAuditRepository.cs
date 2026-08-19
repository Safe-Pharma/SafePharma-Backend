namespace SafePharma.DAL
{
    public interface IAuditRepository:IGenircRepository<Audit>
    {
        Task<IEnumerable<Audit>> GetAuditsWithUsers(Guid pharmacyId);
        Task<Audit?> GetAuditWithUserId(Guid id);
        Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    }
}