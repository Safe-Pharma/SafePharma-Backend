namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {
        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public ITaxRepository TaxRepository { get; }
        Task SaveAsync();
    }
}