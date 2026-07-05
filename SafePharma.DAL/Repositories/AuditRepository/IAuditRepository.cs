namespace SafePharma.DAL
{
    public interface IAuditRepository:IGenircRepository<Audit>
    {
        Task<IEnumerable<Audit>> GetAuditsWithUsers();
        Task<Audit> GetAuditWithUserId(Guid id);

    }
}