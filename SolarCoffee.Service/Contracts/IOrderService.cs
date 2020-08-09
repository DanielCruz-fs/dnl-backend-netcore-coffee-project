using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service.Contracts
{
    public interface IOrderService
    {
        Task<IList<SalesOrder>> GetOrders();
        Task GenerateOpenOrder(SalesOrder order);
        ServiceResponse<bool> MarkFullfilled(int id);
    }
}
