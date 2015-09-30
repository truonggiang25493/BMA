using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class AspNetUserLoginRepository:RepositoryBase<AspNetUserLogin>,IAspNetUserLoginRepository
    {
        public AspNetUserLoginRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IAspNetUserLoginRepository : IRepository<AspNetUserLogin>
    {
        
    }
}

