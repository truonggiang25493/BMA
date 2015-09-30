using System;
using System.Data.Entity;

namespace BMA.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory databaseFactory;
        private BMAEntities dataContext;
        private DbContextTransaction contextTransaction;

        /// <summary>
        /// Constructor create unit of word instance with database factory instance
        /// </summary>
        /// <param name="databaseFactory">Database factory instance</param>
        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public void BeginTransaction()
        {
            contextTransaction = dataContext.Database.BeginTransaction();
        }

        public bool CommitTransaction()
        {
            try
            {
                var rs = DataContext.SaveChanges();
                contextTransaction.Commit();
                return rs > 0;
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
                return false;
            }
        }

        protected BMAEntities DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public bool Commit()
        {
            return DataContext.SaveChanges() > 0;
        }

    }
}
