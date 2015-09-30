using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BMA.Data.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add new entity to database
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);

        /// <summary>
        /// Update an existed entity (which queried in this scope) to database
        /// </summary>
        /// <param name="entity">Updating entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Remove an existed entity (which queried in this scope) 
        /// from database and the managing of current context
        /// </summary>
        /// <param name="entity">Deleting entity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Get entity has primary key value equal input number id
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <returns></returns>
        TEntity GetById(long id);

        /// <summary>
        /// Get entity has primary key value equal input string id
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <returns></returns>
        TEntity GetById(string id);

        /// <summary>
        /// Get first entity which matching with input query
        /// </summary>
        /// <param name="where">Query</param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// Get all entities in database
        /// </summary>
        /// <returns>Enumerable list of result</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Get all entities which matching with input query in database
        /// </summary>
        /// <returns>Enumerable list of result</returns>
        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);
    }
}
