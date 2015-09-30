using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class GuestInfoRepository:RepositoryBase<GuestInfo>,IGuestInfoRepository
    {
        public GuestInfoRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IGuestInfoRepository : IRepository<GuestInfo>
    {
        
    }
}
