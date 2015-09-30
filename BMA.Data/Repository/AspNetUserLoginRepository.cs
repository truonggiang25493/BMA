using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

