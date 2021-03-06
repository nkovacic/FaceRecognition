﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

#endregion

namespace FaceDetection.Library.DataAcccessLayer
{
    sealed class RepositoryQuery<TEntity> : IRepositoryQuery<TEntity> where TEntity : EntityBase
    {
        private readonly List<Expression<Func<TEntity, object>>> _includeProperties;
        private readonly Repository<TEntity> _repository;
        private Expression<Func<TEntity, bool>> _filter;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderByQuerable;
        private int? _page;
        private int? _pageSize;

        public RepositoryQuery(Repository<TEntity> repository, Expression<Func<TEntity, bool>> clause = null)
        {
            _filter = clause;
            _repository = repository;
            _includeProperties = new List<Expression<Func<TEntity, object>>>();
        }

        public RepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderByQuerable = orderBy;
            return this;
        }

        public RepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            _includeProperties.Add(expression);
            return this;
        }

        public IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount)
        {
            _page = page;
            _pageSize = pageSize;
            totalCount = _repository.Select(_filter).Count();

            return _repository.Select(_filter, _orderByQuerable, _includeProperties, _page, _pageSize);
        }

        public IEnumerable<TEntity> Select()
        {
            return _repository.Select(_filter, _orderByQuerable, _includeProperties, _page, _pageSize);
        }

        public async Task<IEnumerable<TEntity>> SingleAsync()
        {
            return await _repository.SelectAsync(_filter, _orderByQuerable, _includeProperties, _page, _pageSize);
        }

        public IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _repository.SelectQuery(query, parameters).AsQueryable();
        }
    }
}