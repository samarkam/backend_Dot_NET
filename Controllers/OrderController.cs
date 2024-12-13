using backend.DTO;
using backend.models;
using backend.models.repository;  // Assurez-vous que l'espace de noms du repository est correct
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        // Injection du repository via le constructeur
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return Ok(orders);  // Retourne toutes les commandes avec un statut 200 OK
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();  // Retourne 404 si la commande n'est pas trouvée
            }

            return Ok(order);  // Retourne la commande avec un statut 200 OK
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderRequestDto orderRequestDto)
        {
            if (orderRequestDto == null)
            {
                return BadRequest();  // Return 400 if the data is invalid
            }

            // Get the User from the database based on the userId
            var user = orderRequestDto.UserId;
            if (user == null)
            {
                return NotFound("User not found");  // Return 404 if the user is not found
            }// Get the User from the database based on the userId
           

            // Create an Order object from the OrderRequestDto (without OrderDetails)
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,  // Set OrderDate to current UTC time
                TotalPrice = orderRequestDto.TotalPrice,
                UserId = orderRequestDto.UserId,  // Set the UserId from the DTO
                User = null,  // Set the User object fetched from the database
                OrderDetails = new List<OrderDetail>() // Initialize empty list to be updated later
            };

            // Add the order to the database (without OrderDetails)
            await _orderRepository.AddOrderAsync(order);

            // Now that the order is created, we can create the OrderDetails
            var orderDetailsList = orderRequestDto.OrderDetails.Select(od => new OrderDetail
            {
                ArticleId = od.ArticleId,
                Article = null,
                Quantity = od.Quantity,
                Price = od.Price,
                Order = order,  // Set the created order to the Order property of each OrderDetail
                OrderId = order.OrderId  // Set the foreign key OrderId
            }).ToList();

            // Add OrderDetails to the database (ensure they are persisted in the OrderDetails table)
            foreach (var orderDetail in orderDetailsList)
            {
                await _orderRepository.AddOrderDetailAsync(orderDetail);  // Assuming you have a method to add OrderDetail
            }

            // Update the Order entity with the created OrderDetails
            order.OrderDetails = orderDetailsList;

            // Return a 201 Created status with the URL of the created order resource
            return Ok("created sucsessful ");  // Return 201 Created with the created order
        }



        // PUT: api/order/5
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
            return Ok(new OrderResponseDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                OrderDetails = orderDetailsDto  // Return as Collection<OrderDetailResponseDto>
            });
        }

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
