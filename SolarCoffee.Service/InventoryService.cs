using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly SolarDbContext context;
        private readonly ILogger<InventoryService> logger;

        public InventoryService(SolarDbContext context, ILogger<InventoryService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Return Snapshot history for the previous 6 hours
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ProductInventorySnapshot>> GetSnapshotHistory()
        {
            // Creating time span example, reference:
            // https://www.dotnetperls.com/timespan
            var earliest = DateTime.Now - TimeSpan.FromHours(6);

            return await this.context.ProductInventorySnapshots.Include(snap => snap.Product)
                                                               .Where(snap => snap.SnapshotTime > earliest && !snap.Product.IsArchived)
                                                               .ToListAsync();
        }

        public async Task<IList<ProductInventory>> GetCurrentInventory()
        {
            return await this.context.ProductInventories.Include(pi => pi.Product)
                                                        .Where(pi => !pi.Product.IsArchived)
                                                        .ToListAsync();
        }

        public async Task<ProductInventory> GetProductById(int id)
        {
            return await this.context.ProductInventories.Include(pi => pi.Product)
                                                        .Where(pi => pi.Product.Id == id)
                                                        .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates number of units available of the provided product id
        /// Adjusts QuantityOnHand by adjustment value
        /// </summary>
        /// <param name="id">productId</param>
        /// <param name="adjustment">number of units added/ removed from inventory</param>
        /// <returns></returns>
        public ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment)
        {
            var now = DateTime.Now;

            try
            {
                var inventory = this.context.ProductInventories.Include(pi => pi.Product)
                                                               .FirstOrDefault(pi => pi.Product.Id == id);
                inventory.QuantityOnHand += adjustment;

                try
                {
                    // unit of work in action
                    this.CreateSnapshot(inventory);
                }
                catch (Exception e)
                {
                    // ILogger prevents from our application to crush but logs some information
                    this.logger.LogError("Error creating inventory snapshot");
                    this.logger.LogError(e.StackTrace);
                }

                // Efcore keeps track of entity fetched by the data base
                this.context.SaveChanges();

                return new ServiceResponse<ProductInventory>()
                {
                    IsSuccess = true,
                    Data = inventory,
                    Message = $"Product: { id } inventory adjusted",
                    Time = now
                };

            }
            catch (Exception e)
            {
                return new ServiceResponse<ProductInventory>()
                {
                    IsSuccess = false,
                    Data = null,
                    Message = $"Product: { id } inventory adjusted failed: { e.InnerException }",
                    Time = now
                };
            }
        }

        private void CreateSnapshot(ProductInventory inventory)
        {
            var snapshot = new ProductInventorySnapshot()
            {
                SnapshotTime = DateTime.Now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };

            // ef core marks add entity based on its type
            // unit of work does the rest
            this.context.Add(snapshot);
        }
    }

}
