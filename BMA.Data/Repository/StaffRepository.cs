using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class StaffRepository : RepositoryBase<Staff>, IStaffRepository
    {
        public StaffRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }


    public interface IStaffRepository : IRepository<Staff>
    {
        
    }
}
