using Microsoft.EntityFrameworkCore;
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
    public class CustomerService : ICustomerService
    {
        private readonly SolarDbContext context;

        public CustomerService(SolarDbContext context)
        {
            this.context = context;
        }
        public ServiceResponse<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                this.context.Customers.Add(customer);
                this.context.SaveChanges();

                return new ServiceResponse<Customer>()
                {
                    IsSuccess = true,
                    Message = "New Customer Added",
                    Time = DateTime.Now,
                    Data = customer
                };

            }
            catch (Exception e)
            {
                return new ServiceResponse<Customer>()
                {
                    IsSuccess = false,
                    Message = e.StackTrace,
                    Time = DateTime.Now,
                    Data = customer
                };
            }
        }

        /// <summary>
        /// Delete a customer record
        /// </summary>
        /// <param name="id">int customer primary key</param>
        /// <returns>ServiceResponse<boool></returns>
        public ServiceResponse<bool> DeleteCustomer(int id)
        {
            var customer = this.context.Customers.Find(id);
            var now = DateTime.Now;
            if (customer == null)
            {
                return new ServiceResponse<bool>() {
                    Time = now,
                    IsSuccess = false,
                    Message = "Customer to delete not found",
                    Data = false
                };
            }

            try
            {
                this.context.Remove(customer);
                this.context.SaveChanges();

                return new ServiceResponse<bool>() {
                    Time = now,
                    IsSuccess = true,
                    Message = "Customer deleted",
                    Data = true
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<bool>() {
                    Time = now,
                    IsSuccess = false,
                    Message = e.InnerException.ToString(),
                    Data = false
                };
            }
        }

        /// <summary>
        /// Gets a customer record by primary key
        /// </summary>
        /// <param name="id">int customer primary key</param>
        /// <returns>customer</returns>
        public async Task<Customer> GetById(int id)
        {
            return await this.context.Customers.FindAsync(id);
        }

        /// <summary>
        /// Returns a list of customers
        /// </summary>
        /// <returns>List<Customer></returns>
        public async Task<IList<Customer>> GetCustomers()
        {
            //return await this.context.Customers.ToListAsync();
            return await this.context.Customers.Include(c => c.Address)
                                               .OrderBy(c => c.LastName)
                                               .ToListAsync();
        }
    }
}
