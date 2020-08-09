using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Serialization
{
    public static class OrderMapper
    {
        /// <summary>
        /// Maps an InvoiceModel view model to a SalesOrder data model
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static SalesOrder SerializeInvoiceToOrder(InvoiceModel invoice)
        {
            var salesOrderItems = invoice.LineItems.Select(item => new SalesOrderItem()
            {
                Id = item.Id,
                Quantity = item.Quantity,
                Product = ProductMapper.SerializationProduct(item.Product)
            }).ToList();

            return new SalesOrder()
            {
                SalesOrderItems = salesOrderItems,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };
        }

        /// <summary>
        /// Maps a collection od SalesOrders (data) to OrderModels (view model)
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static List<OrderModel> SerializeOrdersToViewModel(IEnumerable<SalesOrder> orders)
        {
            return orders.Select(order => new OrderModel() { 
                Id = order.Id,
                CreatedOn = order.CreatedOn,
                UpdatedOn = order.UpdatedOn,
                SalesOrderItems = SerializeSalesOrderItems(order.SalesOrderItems),
                Customer = CustomerMapper.SerializeCustomerModel(order.Customer),
                IsPaid = order.IsPaid
            }).ToList();
        }

        /// <summary>
        /// Maps a collection of SalesOrderItems (data) to SalesOrderItemModels (view model)
        /// </summary>
        /// <param name="orderItems"></param>
        /// <returns></returns>
        private static List<SalesOrderItemModel> SerializeSalesOrderItems(IEnumerable<SalesOrderItem> orderItems)
        {
            return orderItems.Select(item => new SalesOrderItemModel()
            {
                Id = item.Id,
                Quantity = item.Quantity,
                Product = ProductMapper.SerializationProductModel(item.Product)
            }).ToList();
        }
    }
}
