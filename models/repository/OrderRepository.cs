using Microsoft.EntityFrameworkCore;
using backend.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace backend.models.repository
{
   
        public class OrderRepository : IOrderRepository
    {
            private readonly ApplicationDbContext _context;  // Contexte de base de données

            public OrderRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            // Récupérer toutes les commandes
            public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            {
                return await _context.Orders
                                     .Include(o => o.OrderDetails)  // Inclure les détails de la commande
                                     .Include(o => o.User)          // Inclure l'utilisateur associé à la commande
                                     .ToListAsync();
            }

            // Récupérer une commande par son ID
            public async Task<Order> GetOrderByIdAsync(int id)
            {
                return await _context.Orders
                                     .Include(o => o.OrderDetails)  // Inclure les détails de la commande
                                     .Include(o => o.User)          // Inclure l'utilisateur associé à la commande
                                     .FirstOrDefaultAsync(o => o.OrderId == id);
            }

            // Ajouter une nouvelle commande
            public async Task AddOrderAsync(Order order)
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }

            // Mettre à jour une commande existante
            public async Task UpdateOrderAsync(Order order)
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }

            // Supprimer une commande
            public async Task DeleteOrderAsync(int id)
            {
                var order = await _context.Orders.FindAsync(id);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                }
            }

        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            // Ensure that the order detail is associated with a valid order (in case it's not done already)
            if (orderDetail == null)
            {
                throw new ArgumentNullException(nameof(orderDetail));
            }

            // Add the OrderDetail entity to the database
            await _context.OrderDetails.AddAsync(orderDetail);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
    }

