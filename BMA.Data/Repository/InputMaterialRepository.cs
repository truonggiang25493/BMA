using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class InputMaterialRepository:RepositoryBase<InputMaterial>,IInputMaterialRepository
    {
        public InputMaterialRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IInputMaterialRepository : IRepository<InputMaterial>
    {
        
    }
}
