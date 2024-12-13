using System.Collections.Generic;
using System.Threading.Tasks;
using backend.models;

namespace backend.models.repository
{
        public interface IOrderRepository
        {
            Task<IEnumerable<Order>> GetAllOrdersAsync();         // Get all orders
            Task<Order> GetOrderByIdAsync(int orderId);           // Get order by ID
            Task AddOrderAsync(Order order);                       // Add a new order
            Task UpdateOrderAsync(Order order);                    // Update an existing order
            Task DeleteOrderAsync(int orderId);                    // Delete an order
        Task AddOrderDetailAsync(OrderDetail orderDetail);
    }
    

}

