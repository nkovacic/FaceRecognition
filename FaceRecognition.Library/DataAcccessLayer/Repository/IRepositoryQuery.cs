using FaceDetection.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FaceDetection.Library.DataAcccessLayer
{
    interface IRepositoryQuery<TEntity> where TEntity : EntityBase
    {
        RepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        RepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount);
        IEnumerable<TEntity> Select();
        Task<IEnumerable<TEntity>> SingleAsync();
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
    }
}