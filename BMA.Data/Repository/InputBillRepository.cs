using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class InputBillRepository:RepositoryBase<InputBill>,IInputBillRepository
    {
        public InputBillRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IInputBillRepository : IRepository<InputBill>
    {
        
    }
}
