using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class ProductMarerialRepository:RepositoryBase<ProductMaterial>,IProductMaterialRepository
    {
        public ProductMarerialRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IProductMaterialRepository : IRepository<ProductMaterial>
    {
        
    }
}
