using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }



    public interface ICustomerRepository : IRepository<Customer>
    {
    }
}
