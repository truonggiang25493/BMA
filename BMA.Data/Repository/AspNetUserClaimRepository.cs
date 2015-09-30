using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class AspNetUserClaimRepository : RepositoryBase<AspNetUserClaim>, IAspNerUserClaimReponsitory
    {
        public AspNetUserClaimRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }


    public interface IAspNerUserClaimReponsitory : IRepository<AspNetUserClaim>
    {
        
    }
}
