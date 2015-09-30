using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class TaxTypeRepository:RepositoryBase<TaxType>,ITaxTypeRepository
    {
        public TaxTypeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface ITaxTypeRepository : IRepository<TaxType>
    {
        
    }
}
