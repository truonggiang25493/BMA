using BMA.Data.Infrastructure;

namespace BMA.Data.Repository
{
    public class OrderItemRepository:RepositoryBase<OrderItem>,IOrderItemRepository
    {
        public OrderItemRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        
    }
}
