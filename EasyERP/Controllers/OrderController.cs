using EasyERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public OrderController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //API 1 create order
        [HttpPost]
        [Route("CreateOrders")]
        public async Task<IActionResult> CreateOrders([FromBody] Order request){
            var product = await _context.products.FindAsync(request.IntProductId);
            if (product== null || product.NumStock < request.NumQuantity)
            {
                return BadRequest("Insufficient stock.");
            }
            var order = new Order
            {
                IntProductId = request.IntProductId,
                StrCustomerName = request.StrCustomerName,
                NumQuantity = request.NumQuantity,
                DtOrderDate = DateTime.UtcNow
            };

            product.NumStock -= request.NumQuantity;
            _context.orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        //API 2 update order quantity
        [HttpPut]
        [Route("update-order/{orderId}")]
        public async Task<IActionResult> UpdateOrderQuantity(int orderId, [FromBody] UpdateOrderDTO request)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var product = await _context.products.FindAsync(order.IntProductId);
            if (product == null) return NotFound();

            var stock = request.NewQuantity - order.NumQuantity;

            if (product.NumStock < stock)
                return BadRequest("Insufficient stock for the update.");

            product.NumStock -= stock;
            order.NumQuantity = request.NewQuantity;

            await _context.SaveChangesAsync();
            return Ok(order);
        }
        //API 3 delete order
        [HttpDelete]
        [Route("delete-order/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var product = await _context.products.FindAsync(order.IntProductId);
            if (product != null)
                product.NumStock += order.NumQuantity;

            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //API 4 fetch all order and product details
        [HttpGet]
        [Route("Orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.orders.Include(o => o.Product).Select(o => new
            {
                o.IntProductId,
                o.StrCustomerName,
                o.NumQuantity,
                o.DtOrderDate,
                ProductName = o.Product.StrProductName,
                UnitPrice = o.Product.NumUnitPrice

            }).ToListAsync();

            return Ok(orders);
        }

        //API 5 product summary
        [HttpGet]
        [Route("product-summary")]
        public async Task<IActionResult> GetProductSummary()
        {
            var summary = await _context.orders
                .GroupBy(o => o.IntProductId)
                .Select(g => new
                {
                    ProductName = g.First().Product.StrProductName,
                    TotalQuantity = g.Sum(o => o.NumQuantity),
                    TotalRevenue = g.Sum(o => o.NumQuantity * o.Product.NumUnitPrice)
                })
                .ToListAsync();

            return Ok(summary);
        }
        //API 6
        [HttpGet]
        [Route("low-stock/{threshold}")]
        public async Task<IActionResult> GetLowStockProducts(int threshold)
        {
            var products = await _context.products
                .Where(p => p.NumStock < threshold)
                .Select(p => new
                {
                    p.StrProductName,
                    p.NumUnitPrice,
                    p.NumStock
                })
                .ToListAsync();

            return Ok(products);
        }

        //API 7
        [HttpGet]
        [Route("top-customers")]
        public async Task<IActionResult> GetTopCustomers()
        {
            var customers = await _context.orders
                .GroupBy(o => o.StrCustomerName)
                .OrderByDescending(g => g.Sum(o => o.NumQuantity))
                .Take(3)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalQuantity = g.Sum(o => o.NumQuantity)
                })
                .ToListAsync();

            return Ok(customers);
        }

        //API 8
        [HttpGet]
        [Route("not-ordered-products")]
        public async Task<IActionResult> GetNotOrderedProducts()
        {
            var products = await _context.products
                .Where(p => !_context.orders.Any(o => o.IntProductId == p.IntProductId))
                .Select(p => new
                {
                    p.StrProductName,
                    p.NumUnitPrice,
                    p.NumStock
                })
                .ToListAsync();

            return Ok(products);
        }

        // API 9
        [HttpPost]
        [Route("bulk-orders")]
        public async Task<IActionResult> BulkCreateOrders([FromBody] List<Order> requests)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var request in requests)
                {
                    var product = await _context.products.FindAsync(request.IntProductId);

                    if (product == null || product.NumStock < request.NumQuantity)
                        throw new Exception($"Insufficient stock for ProductId {request.IntProductId}");

                    var order = new Order
                    {
                        IntProductId = request.IntProductId,
                        StrCustomerName = request.StrCustomerName,
                        NumQuantity = request.NumQuantity,
                        DtOrderDate = DateTime.UtcNow
                    };

                    product.NumStock -= request.NumQuantity;
                    _context.orders.Add(order);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Orders created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }


    }
}
