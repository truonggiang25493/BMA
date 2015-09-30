using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class ProductRepository:RepositoryBase<Product>,IProductRepository
    {
        public ProductRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IProductRepository : IRepository<Product>
    {
        
    }
}
