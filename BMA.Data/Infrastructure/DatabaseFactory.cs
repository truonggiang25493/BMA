namespace BMA.Data.Infrastructure
{
    class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private BMAEntities dataContext;

        /// <summary>
        /// Get instance of BMA db context in this scope
        /// </summary>
        /// <returns>BMA db_context</returns>
        public BMAEntities Get()
        {
            var entity = new BMAEntities();
            return dataContext ?? (dataContext = entity);
        }

        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
