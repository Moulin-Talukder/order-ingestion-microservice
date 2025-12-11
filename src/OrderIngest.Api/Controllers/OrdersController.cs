using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using OrderIngest.Api.Data;
using OrderIngest.Api.Models;
using OrderIngest.Api.Services;

namespace OrderIngest.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly ILogger<OrdersController> _logger;
        private readonly ILogisticsService _logistics;

        public OrdersController(IDbConnectionFactory dbFactory, ILogger<OrdersController> logger, ILogisticsService logistics)
        {
            _dbFactory = dbFactory;
            _logger = logger;
            _logistics = logistics;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
        {
            if (!Request.Headers.TryGetValue("Idempotency-Key", out var idemp) || string.IsNullOrWhiteSpace(idemp))
                return BadRequest(new { code = "ERR_1001", message = "Missing Idempotency-Key header" });

            if (dto.Customer == null || string.IsNullOrWhiteSpace(dto.Customer.Email))
                return BadRequest(new { code = "ERR_1002", message = "Invalid or missing customer email" });

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(new { code = "ERR_1006", message = "Order must contain items" });

            foreach (var it in dto.Items)
            {
                if (it.Quantity <= 0)
                    return BadRequest(new { code = "ERR_1003", message = "Quantity must be > 0" });
                if (it.UnitPrice <= 0)
                    return BadRequest(new { code = "ERR_1004", message = "UnitPrice must be > 0" });
            }

            var computed = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
            if (computed != dto.TotalAmount)
                return BadRequest(new { code = "ERR_1005", message = "TotalAmount mismatch with items" });

            var orderId = dto.OrderId == Guid.Empty ? Guid.NewGuid() : dto.OrderId;

            using var conn = _dbFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@RequestId", idemp.ToString());
            parameters.Add("@OrderId", orderId);
            parameters.Add("@CustomerEmail", dto.Customer.Email);
            parameters.Add("@CustomerName", dto.Customer.Name);
            parameters.Add("@Currency", dto.Currency ?? "USD");
            parameters.Add("@TotalAmount", dto.TotalAmount);

            var dt = new DataTable();
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            foreach (var it in dto.Items) 
                dt.Rows.Add(it.SKU, it.ProductName ?? "", it.Quantity, it.UnitPrice);

            parameters.Add("@OrderItems", dt.AsTableValuedParameter("OrderItemType"));
            parameters.Add("@OutStatus", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.OpenAsync();
            await conn.ExecuteAsync("sp_InsertOrder", parameters, commandType: CommandType.StoredProcedure);

            var outStatus = parameters.Get<int>("@OutStatus");

            if (outStatus == 1)
                return Ok(new { message = "Already processed", orderId });

            if (outStatus == 0)
            {
                _ = _logistics.NotifyLogisticsAsync(orderId);
                return CreatedAtAction(nameof(GetPlaceholder), new { id = orderId }, new { orderId });
            }

            return StatusCode(500, new { code = "ERR_1500", message = "Failed to insert order" });
        }

        [HttpGet("{id}")]
        public IActionResult GetPlaceholder(Guid id) => Ok(new { orderId = id });
    }
}
