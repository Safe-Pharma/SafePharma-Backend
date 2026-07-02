using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SafePharma.DAL
{
    public class GenircRepository<T> : IGenircRepository<T> where T : class
    {
        protected readonly AppDbContext _db;

        public GenircRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _db.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<T> GetById(Guid id)
        {
            return await _db.Set<T>().FindAsync(id);
        }
        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
        }



        public async Task<IEnumerable<T>> GetAllWithException(
            Expression<Func<T, bool>>? exceptionExpression = null,
            bool isTracking = false
            )
        {
            IQueryable<T> query = _db.Set<T>();

            if (exceptionExpression is not null)
            {
                query = query.Where(exceptionExpression);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();

            }
            return await query.ToListAsync();

        }

    }
}
