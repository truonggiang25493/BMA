using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class TaxRateRepository:RepositoryBase<TaxRate>,ITaxRateRepository
    {
        public TaxRateRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface ITaxRateRepository : IRepository<TaxRate>
    {
        
    }
}
