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
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> logger;
        private readonly IInventoryService inventoryService;

        public InventoryController(ILogger<InventoryController> logger, IInventoryService inventoryService)
        {
            this.logger = logger;
            this.inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentInventory()
        {
            this.logger.LogInformation("Getting all inventory");
            var inventory = await this.inventoryService.GetCurrentInventory();

            inventory.Select(pi => new ProductInventoryModel()
            {
                Id = pi.Id,
                Product = ProductMapper.SerializationProductModel(pi.Product),
                IdealQuantity = pi.IdealQuantity,
                QuantityOnHand = pi.QuantityOnHand
            }).OrderBy(pi => pi.Product.Name).ToList();

            return Ok(inventory);
        }

        [HttpPut]
        public IActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
 
            this.logger.LogInformation("Updating Inventory " +
                                       $"for { id } - " +
                                       $"Adjustment: { adjustment }");

            var inventory = this.inventoryService.UpdateUnitsAvailable(id, adjustment);
            return Ok(inventory);
        }

        [HttpGet("snapshot")]
        public async Task<IActionResult> GetSnapshotHistory()
        {
            /**
             * THIS ONLY WORKS FOR CATEGORIES AS LABELS: 
             * { 
             * timeline: [1, 2, 3, 4, ..., n], (array of dates)
             * inventory: [{id: 1, qty: [12, 34, 45, 34, ..., n]}, {id: 2, qty: [12, 34, 45, 34, ..., n]}] (every qty for each date)
             * }
             **/
            this.logger.LogInformation("Getting Snapshot History");

            /**
             * THIS STRUCTURED WORKS FOR "DATETIME" IN APEXCHARTS AS LABELS IN XAXIS
             * 
             **/
            try
            {
                var snapshotHistory = await this.inventoryService.GetSnapshotHistory();

                //Get distinct points in time a snapshot was collected
                //var timelineMarkers = snapshotHistory.Select(hist => hist.SnapshotTime)
                //                                     .Distinct()
                //                                     .ToList();

                // Get quantities grouped by id
                //var snapshots = snapshotHistory.GroupBy(hist => hist.Product, hist => hist.QuantityOnHand,
                //                                             (key, g) => new ProductInventorySnapshotModel()
                //                                             {
                //                                                 ProductId = key.Id,
                //                                                 QuantityOnHand = g.ToList()
                //                                             }).OrderBy(hist => hist.ProductId).ToList();

                //var viewModel = new SnapshotResponse()
                //{
                //    TimeLine = timelineMarkers,
                //    ProductInventorySnapshots = snapshots
                //};

                //return Ok(viewModel);

                var snapshots = snapshotHistory.GroupBy(hist => hist.Product)
                                               .Select( g => new { 
                                                   ProductId = g.Key.Id,
                                                   Data = g.ToList()
                                               });
                return Ok(snapshots);
            }
            catch (Exception e)
            {
                this.logger.LogError("Error getting snapshot history");
                this.logger.LogError(e.StackTrace);
                return BadRequest("Error retrieving history");
            }
        }
    }
}
