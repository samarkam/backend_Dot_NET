
using backend.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace backend.models.repository
{
    

        public class OrderDetailRepository : IOrderDetailRepository
        {
            private readonly ApplicationDbContext _context;  // Assuming your DbContext is ApplicationDbContext

            public OrderDetailRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<OrderDetail>> GetAllAsync()
            {
                return await _context.OrderDetails
                                     .Include(od => od.Order)  // Include related Order
                                     .Include(od => od.Article) // Include related Article
                                     .ToListAsync();
            }

            public async Task<OrderDetail> GetByIdAsync(int id)
            {
                return await _context.OrderDetails
                                     .Include(od => od.Order)
                                     .Include(od => od.Article)
                                     .FirstOrDefaultAsync(od => od.OrderDetailId == id);
            }

            public async Task AddAsync(OrderDetail orderDetail)
            {
                await _context.OrderDetails.AddAsync(orderDetail);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(OrderDetail orderDetail)
            {
                _context.OrderDetails.Update(orderDetail);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var orderDetail = await _context.OrderDetails.FindAsync(id);
                if (orderDetail != null)
                {
                    _context.OrderDetails.Remove(orderDetail);
                    await _context.SaveChangesAsync();
                }
            }
        }
    

}
