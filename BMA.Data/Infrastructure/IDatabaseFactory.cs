using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMA.Data.Infrastructure
{
    public interface IDatabaseFactory:IDisposable
    {
        /// <summary>
        /// Get db_context for this scope
        /// </summary>
        /// <returns>Database context</returns>
        BMAEntities Get();
    }
}
