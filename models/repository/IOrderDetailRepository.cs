using System.Collections.Generic;
using System.Threading.Tasks;
namespace backend.models.repository
{
        public interface IOrderDetailRepository
        {
            Task<IEnumerable<OrderDetail>> GetAllAsync();
            Task<OrderDetail> GetByIdAsync(int id);
            Task AddAsync(OrderDetail orderDetail);
            Task UpdateAsync(OrderDetail orderDetail);
            Task DeleteAsync(int id);
        }
   
}
