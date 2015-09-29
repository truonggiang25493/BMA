using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMA.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        bool Commit();
        void BeginTransaction();
        bool CommitTransaction();
    }
}
