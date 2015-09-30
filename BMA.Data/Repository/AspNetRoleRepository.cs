using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class AspNetRoleRepository:RepositoryBase<AspNetRole>,IAspNetRoleRepository
    {
        public AspNetRoleRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IAspNetRoleRepository : IRepository<AspNetRole>
    {
        
    }
}
