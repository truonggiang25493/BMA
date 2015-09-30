using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class OrderRepository:RepositoryBase<Order>,IOrderRepository
    {
        public OrderRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IOrderRepository : IRepository<Order>
    {
        
    }
}
