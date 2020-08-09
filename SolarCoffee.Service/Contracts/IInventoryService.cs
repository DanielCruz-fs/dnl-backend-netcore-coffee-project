using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service.Contracts
{
    public interface IInventoryService
    {
        Task<IList<ProductInventory>> GetCurrentInventory();
        ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment);
        Task<ProductInventory> GetProductById(int id);
        Task<IList<ProductInventorySnapshot>> GetSnapshotHistory();
    }
}
