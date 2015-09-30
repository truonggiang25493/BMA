using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class OutputBillRepository:RepositoryBase<OutputBill>,IOutputBillRepository
    {
        public OutputBillRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IOutputBillRepository : IRepository<OutputBill>
    {
        
    }
}
