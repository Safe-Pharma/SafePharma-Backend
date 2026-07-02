using System.Linq.Expressions;

namespace SafePharma.DAL
{
    public interface IGenircRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllWithException(Expression<Func<T, bool>>? exceptionExpression = null, bool isTracking = false);
        Task<T> GetById(Guid id);
    }
}