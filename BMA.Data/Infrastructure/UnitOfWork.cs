using System;
using System.Data.Entity;

namespace BMA.Data.Infrastructure
{
<<<<<<< HEAD
    public class UnitOfWork
=======
    public class UnitOfWork : IUnitOfWork
>>>>>>> master
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

<<<<<<< HEAD
        /// <summary>
        /// Create new transaction to explance unit of work
        /// </summary>
=======
>>>>>>> master
        public void BeginTransaction()
        {
            contextTransaction = dataContext.Database.BeginTransaction();
        }

<<<<<<< HEAD
        protected BMAEntities DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public bool Commit()
        {
            return DataContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Try to execute all changings of database which create in this unit of work
        /// </summary>
        /// <returns>true if execute successfull</returns>
=======
>>>>>>> master
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
<<<<<<< HEAD
=======

        protected BMAEntities DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public bool Commit()
        {
            return DataContext.SaveChanges() > 0;
        }

>>>>>>> master
    }
}
