using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BMA.Data.Infrastructure
{
    public class RepositoryBase<TEntity> where TEntity : class
    {
        private BMAEntities dataContext;
        protected readonly IDbSet<TEntity> dbSet;

        /// <summary>
        /// Database factory of this repository
        /// </summary>
        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        /// <summary>
        /// Current db_context from database factory
        /// </summary>
        protected BMAEntities DataContext
        {
            get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
        }

        /// <summary>
        /// Constructor for create repository with database factory instance
        /// </summary>
        /// <param name="databaseFactory">Database factory instance</param>
        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbSet = DataContext.Set<TEntity>();
        }

        /// <summary>
        /// Add new entity to database
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Update an existed entity (which queried in this scope) to database
        /// </summary>
        /// <param name="entity">Updating entity</param>
        public virtual void Update(TEntity entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Remove an existed entity (which queried in this scope) 
        /// from database and the managing of current context
        /// </summary>
        /// <param name="entity">Deleting entity</param>
        public virtual void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = dbSet.Where(where).AsEnumerable();
            foreach (TEntity obj in objects)
            {
                dbSet.Remove(obj);
            }
        }

        /// <summary>
        /// Get entity has primary key value equal input number id
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <returns></returns>
        public virtual TEntity GetById(long id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Get entity has primary key value equal input string id
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <returns></returns>
        public virtual TEntity GetById(string id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Get all entities in database
        /// </summary>
        /// <returns>Enumerable list of result</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet;
        }

        /// <summary>
        /// Get all entities which matching with input query in database
        /// </summary>
        /// <returns>Enumerable list of result</returns>
        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return dbSet.Where(where);
        }

        /// <summary>
        /// Get first entity which matching with input query
        /// </summary>
        /// <param name="where">Query</param>
        /// <returns></returns>
        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault();
        }
    }
}
