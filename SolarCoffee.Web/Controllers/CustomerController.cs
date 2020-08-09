using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data.Models;
using SolarCoffee.Service.Contracts;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<Customer> logger;
        private readonly ICustomerService customerService;

        public CustomerController(ILogger<Customer> logger, ICustomerService customerService)
        {
            this.logger = logger;
            this.customerService = customerService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            this.logger.LogInformation("Getting Customers");
            var customers = await this.customerService.GetCustomers();

            var customersModels = customers.Select(customer => new CustomerModel()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PrimaryAddress = CustomerMapper.MapCustomerAddress(customer.Address),
                CreatedOn = customer.CreatedOn,
                UpdatedOn = customer.UpdatedOn
            }).OrderByDescending(c => c.CreatedOn).ToList();

            return Ok(customersModels);
        }

        [HttpPost()]
        public IActionResult CreateCustomer([FromBody] CustomerModel customer)
        {
            this.logger.LogInformation("Creating a new customer");
            var now = DateTime.Now;

            customer.CreatedOn = now;
            customer.UpdatedOn = now;
            var customerData = CustomerMapper.SerializeCustomer(customer);
            var newCustomer = this.customerService.CreateCustomer(customerData);
            return Ok(newCustomer);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            this.logger.LogInformation("Deleting Customer");
            var response = this.customerService.DeleteCustomer(id);
            return Ok(response);
        }
    }
}
