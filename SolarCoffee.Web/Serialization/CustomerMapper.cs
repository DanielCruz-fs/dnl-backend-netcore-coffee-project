using Microsoft.AspNetCore.Http;
using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Serialization
{
    public static class CustomerMapper
    {
        /// <summary>
        /// Serialize a customer data model into a CustomerMode view model
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static CustomerModel SerializeCustomerModel(Customer customer)
        {
            var address = new CustomerAddressModel()
            {
                Id = customer.Id,
                AddressLine1 = customer.Address.AddressLineOne,
                AddressLine2 = customer.Address.AddressLineTwo,
                City = customer.Address.City,
                State = customer.Address.State,
                Country = customer.Address.Country,
                PostalCode = customer.Address.PostalCode,
                CreatedOn = customer.Address.CreatedOn,
                UpdatedOn = customer.Address.UpdatedOn
            };

            return new CustomerModel()
            {
                Id = customer.Id,
                CreatedOn = customer.CreatedOn,
                UpdatedOn = customer.UpdatedOn,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PrimaryAddress = address
            };
        } 
        
        /// <summary>
        /// Serialize a customerModel view model into a Customer data model
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static Customer SerializeCustomer(CustomerModel customer)
        {
            var address = new Address()
            {
                Id = customer.Id,
                AddressLineOne = customer.PrimaryAddress.AddressLine1,
                AddressLineTwo = customer.PrimaryAddress.AddressLine2,
                City = customer.PrimaryAddress.City,
                State = customer.PrimaryAddress.State,
                Country = customer.PrimaryAddress.Country,
                PostalCode = customer.PrimaryAddress.PostalCode,
                CreatedOn = customer.PrimaryAddress.CreatedOn,
                UpdatedOn = customer.PrimaryAddress.UpdatedOn
            };

            return new Customer()
            {
                Id = customer.Id,
                CreatedOn = customer.CreatedOn,
                UpdatedOn = customer.UpdatedOn,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = address
            };
        }

        public static CustomerAddressModel MapCustomerAddress(Address address)
        {
            return new CustomerAddressModel()
            {
                Id = address.Id,
                AddressLine1 = address.AddressLineOne,
                AddressLine2 = address.AddressLineTwo,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                CreatedOn = address.CreatedOn,
                UpdatedOn = address.UpdatedOn
            };
        }
    }
}