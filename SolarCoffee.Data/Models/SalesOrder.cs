using System;
using System.Collections.Generic;
using System.Text;

namespace SolarCoffee.Data.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public bool IsPaid  { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public Customer Customer { get; set; }
        public ICollection<SalesOrderItem> SalesOrderItems { get; set; }
    }
}
