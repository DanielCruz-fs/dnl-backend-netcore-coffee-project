using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service.Contracts
{
    public interface ICustomerService
    {
        Task<IList<Customer>> GetCustomers();
        ServiceResponse<Customer> CreateCustomer(Customer customer);
        ServiceResponse<bool> DeleteCustomer(int id);
        Task<Customer> GetById(int id);
    }
}
