using backend.models.repository;
using backend.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class OrderDetailsController : ControllerBase
        {
            private readonly IOrderDetailRepository _orderDetailRepository;

            public OrderDetailsController(IOrderDetailRepository orderDetailRepository)
            {
                _orderDetailRepository = orderDetailRepository;
            }

            // GET: api/orderdetails
            [HttpGet]
            public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
            {
                var orderDetails = await _orderDetailRepository.GetAllAsync();
                return Ok(orderDetails);
            }

            // GET: api/orderdetails/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
            {
                var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

                if (orderDetail == null)
                {
                    return NotFound();
                }

                return Ok(orderDetail);
            }

            // POST: api/orderdetails
            [HttpPost]
            public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
            {
                await _orderDetailRepository.AddAsync(orderDetail);
                return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailId }, orderDetail);
            }

            // PUT: api/orderdetails/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
            {
                if (id != orderDetail.OrderDetailId)
                {
                    return BadRequest();
                }

                await _orderDetailRepository.UpdateAsync(orderDetail);

                return NoContent();
            }

            // DELETE: api/orderdetails/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteOrderDetail(int id)
            {
                await _orderDetailRepository.DeleteAsync(id);
                return NoContent();
            }
        }
    
}

