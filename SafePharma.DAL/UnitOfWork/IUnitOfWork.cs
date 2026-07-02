namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}