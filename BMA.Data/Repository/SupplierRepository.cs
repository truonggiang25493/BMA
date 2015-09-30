using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class SupplierRepository:RepositoryBase<Supplier>,ISupplierRepository
    {
        public SupplierRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface ISupplierRepository : IRepository<Supplier>
    {
        
    }
}
