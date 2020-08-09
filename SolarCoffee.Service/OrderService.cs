using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service
{
    public class OrderService : IOrderService
    {
        private readonly SolarDbContext context;
        private readonly ILogger<OrderService> logger;
        private readonly IProductService productService;
        private readonly IInventoryService inventoryService;

        public OrderService(SolarDbContext context, ILogger<OrderService> logger,
                            IProductService productService, IInventoryService inventoryService)
        {
            this.context = context;
            this.logger = logger;
            this.productService = productService;
            this.inventoryService = inventoryService;
        }
        public async Task GenerateOpenOrder(SalesOrder order)
        {
            this.logger.LogInformation("Generating new order");

            foreach (var item in order.SalesOrderItems)
            {
                item.Product = await this.productService.GetProductById(item.Product.Id);
                var inventory = await this.inventoryService.GetProductById(item.Product.Id);
                this.inventoryService.UpdateUnitsAvailable(inventory.Id, -item.Quantity);
            }

            this.context.SalesOrders.Add(order);
            await this.context.SaveChangesAsync();

            this.logger.LogInformation($"New order created: { order.Id }");
        }

        public async Task<IList<SalesOrder>> GetOrders()
        {
            return await this.context.SalesOrders.Include(so => so.Customer)
                                                    .ThenInclude(c => c.Address)
                                                 .Include(so => so.SalesOrderItems)
                                                    .ThenInclude(item => item.Product)
                                                 .ToListAsync();
        }

        /// <summary>
        /// Marks an open SsalesOrder as paid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<bool> MarkFullfilled(int id)
        {
            var now = DateTime.Now;
            var order = this.context.SalesOrders.Find(id);
            order.UpdatedOn = now;
            order.IsPaid = true;

            try
            {
                this.context.Update(order);
                this.context.SaveChanges();

                return new ServiceResponse<bool>()
                {
                    IsSuccess = true,
                    Data = true,
                    Message = $"Order { order.Id } closed: Invoice paid in full",
                    Time = now
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<bool>()
                {
                    IsSuccess = false,
                    Data = false,
                    Message = e.StackTrace,
                    Time = now
                };
            }
        }
    }
}
