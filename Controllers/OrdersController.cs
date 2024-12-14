using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.models;
using backend.DTO.order;
using backend.REPOSITORY;
using System.Collections.ObjectModel;
using backend.DTO.user;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderWithUserResponseDto>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Fetch paginated orders with their details, articles, and user profiles
            var totalOrders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Article)
                .Include(o => o.User)
                .Include(o => o.User.UserProfile)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalOrdersCount = await _context.Orders.CountAsync();

            // Map orders to OrderWithUserResponseDto
            var orderResponseDtos = totalOrders.Select(order => new OrderWithUserResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                UserDetails = new UserDetailsDto
                {
                    UserName = order.User.UserName,
                    Address = order.User.UserProfile?.Address,
                    PhoneNumber = order.User.UserProfile?.PhoneNumber,
                    UserId = order.User.UserId,
                    Email = order.User.Email
                    
                },
                OrderDetails = new Collection<OrderDetailResponseDto>(
                    order.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ArticleId = od.ArticleId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        Article = new ArticleOrderDetailResponseDto
                        {
                            ArticleId = od.Article.ArticleId,
                            Name = od.Article.Name,
                            Price = od.Article.Price
                        }
                    }).ToList()
                )
            }).ToList();

            return Ok(new { data = orderResponseDtos, totalOrders = totalOrdersCount });
        }



        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Article)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            var orderResponseDto = new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                OrderDetails = new Collection<OrderDetailResponseDto>(
                    order.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ArticleId = od.ArticleId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        Article = new ArticleOrderDetailResponseDto
                        {
                            ArticleId = od.Article.ArticleId,
                            Name = od.Article.Name,
                            Price = od.Article.Price
                        }
                    }).ToList()
                )
            };

            return Ok(orderResponseDto);
        }


        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(OrderRequestDto orderRequestDto)
        {
            if (orderRequestDto == null)
            {
                return BadRequest(); 
            }

            //-------------------------------------------------
            var user = await _context.Users.FirstOrDefaultAsync(o => o.UserId == orderRequestDto.UserId);  
            if (user == null)
            {
                return NotFound("User not found");  
            }
            //-------------------------------------------------

            var articleIdsInOrder = orderRequestDto.OrderDetails
                .Select(orderDetail => orderDetail.ArticleId)
                .ToList();

            var existingArticleIds = _context.Articles
                .Where(article => articleIdsInOrder.Contains(article.ArticleId))
                .Select(article => article.ArticleId)
                .ToList();

            bool allExist = articleIdsInOrder.All(id => existingArticleIds.Contains(id));

            if (!allExist)
            {
                throw new Exception("Some articles in the order do not exist in the database.");
            }

            //-------------------------------------------------
            var order = new Order
            {
                OrderDate = DateTime.UtcNow, 
                TotalPrice = orderRequestDto.TotalPrice,
                UserId = user.UserId,  
                User = user, 
                OrderDetails = new List<OrderDetail>() 
            };

            await _orderRepository.AddOrderAsync(order);

            //-------------------------------------------------

            var orderDetailsList = new List<OrderDetail>();

            foreach (var od in orderRequestDto.OrderDetails)
            {
                var article = await _context.Articles.FirstOrDefaultAsync(o => o.ArticleId == od.ArticleId);
                if (article == null)
                {
                    throw new Exception($"Article with ID {od.ArticleId} does not exist.");
                }

                var orderDetail = new OrderDetail
                {
                    ArticleId = od.ArticleId,
                    Article = article, 
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Order = order,     
                    OrderId = order.OrderId  
                };

                await _orderRepository.AddOrderDetailAsync(orderDetail); 
            }

            //-------------------------------------------------

            order.OrderDetails = orderDetailsList;

            //-------------------------------------
            var orderResponseDto = new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                OrderDetails = new Collection<OrderDetailResponseDto>(
                   order.OrderDetails.Select(od => new OrderDetailResponseDto
                   {
                       OrderDetailId = od.OrderDetailId,
                       ArticleId = od.ArticleId,
                       Quantity = od.Quantity,
                       Price = od.Price,
                       Article = new ArticleOrderDetailResponseDto
                       {
                           ArticleId = od.Article.ArticleId,
                           Name = od.Article.Name,
                           Price = od.Article.Price
                       }
                   }).ToList()
               )
            };

            return Ok(orderResponseDto);

        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrdersByUserId(int userId)
        {
            // Fetch orders by user ID with their details and articles
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Article)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for user with ID {userId}.");
            }

            // Map orders to OrderResponseDto
            var orderResponseDtos = orders.Select(order => new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                OrderDetails = new Collection<OrderDetailResponseDto>(
                    order.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ArticleId = od.ArticleId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        Article = new ArticleOrderDetailResponseDto
                        {
                            ArticleId = od.Article.ArticleId,
                            Name = od.Article.Name,
                            Price = od.Article.Price
                        }
                    }).ToList()
                )
            }).ToList();

            return Ok(orderResponseDtos);
        }



        /* // PUT: api/order/5
         [HttpPut("{id}")]
         public async Task<IActionResult> UpdateOrder(int id, OrderRequestDto OrderRequestDto)
         {


             var order = await _orderRepository.GetOrderByIdAsync(id);
             if (order == null)
             {
                 return NotFound();  // Retourne 404 si la commande n'existe pas
             }

             // Met à jour la commande
             await _orderRepository.UpdateOrderAsync(order);

             var orderDetailsDto = new Collection<OrderDetailResponseDto>(
                     order.OrderDetails.Select(od => new OrderDetailResponseDto
                     {
                         OrderDetailId = od.OrderDetailId,
                         ArticleId = od.ArticleId,
                         Quantity = od.Quantity,
                         Price = od.Price,
                     }).ToList() // Convert to List<OrderDetailResponseDto> before wrapping in Collection
                 );

             // Return the updated order response
             return Ok(new OrderResponseDto
             {
                 OrderId = order.OrderId,
                 OrderDate = order.OrderDate,
                 TotalPrice = order.TotalPrice,
                 OrderDetails = orderDetailsDto  // Return as Collection<OrderDetailResponseDto>
             });
         }*/

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();  // Retourne 404 si la commande n'existe pas
            }

            // Supprime la commande
            await _orderRepository.DeleteOrderAsync(id);

            return NoContent();  // Retourne 204 No Content après une suppression réussie
        }
    }
}
