using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class CategoryRepository:RepositoryBase<Category>,ICategoryRepository
    {
        public CategoryRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        
    }
}
