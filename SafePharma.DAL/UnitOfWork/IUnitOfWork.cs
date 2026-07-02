namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {
        public IPharmacySettingRepository PharmacySettingRepository { get; }
        Task SaveAsync();
    }
}