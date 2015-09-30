using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class AspNetUserRepository : RepositoryBase<AspNetUser>, IAspNetUserRepository
    {
        public AspNetUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IAspNetUserRepository : IRepository<AspNetUser>
    {

    }
}
