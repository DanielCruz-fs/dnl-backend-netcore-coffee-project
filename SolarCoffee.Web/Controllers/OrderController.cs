using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Service.Contracts;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly ICustomerService customerService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, ICustomerService customerService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.customerService = customerService;
        }

        [HttpPost("invoice")]
        public async Task<IActionResult> GenerateNewOrder([FromBody] InvoiceModel invoice)
        {
            this.logger.LogInformation("Generating Invoice");
            var order = OrderMapper.SerializeInvoiceToOrder(invoice);
            order.Customer = await this.customerService.GetById(invoice.CustomerId);
            await this.orderService.GenerateOpenOrder(order);
            return Ok();
        }

        [HttpGet()]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await this.orderService.GetOrders();
            var orderModels = OrderMapper.SerializeOrdersToViewModel(orders);
            return Ok(orderModels);
        }

        [HttpPut("complete/{id}")]
        public IActionResult MarkOrderComplete(int id)
        {
            this.logger.LogInformation($"Marking order { id } complete");
            this.orderService.MarkFullfilled(id);
            return Ok();
        }
    }
}
