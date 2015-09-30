using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class RecipeRepository:RepositoryBase<Recipe>,IRecipeRepository
    {
        public RecipeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IRecipeRepository : IRepository<Recipe>
    {
        
    }
}
