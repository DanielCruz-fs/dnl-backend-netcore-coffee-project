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
    public class ProductService : IProductService
    {
        private readonly SolarDbContext context;

        public ProductService(SolarDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Archives a product by setting boolean IsArchived to true
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<Product> ArchiveProduct(int id)
        {
            try
            {
                var product = this.context.Products.FirstOrDefault(p => p.Id == id);
                product.IsArchived = true;
                this.context.SaveChanges();

                return new ServiceResponse<Product>()
                {
                    Data = product,
                    Time = DateTime.UtcNow,
                    Message = "Archived product",
                    IsSuccess = true
                };

            }
            catch (Exception e)
            {
                return new ServiceResponse<Product>()
                {
                    Data = null,
                    Time = DateTime.UtcNow,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }
        
        /// <summary>
        /// Adds a new product to the Database
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ServiceResponse<Product> CreateProduct(Product product)
        {
            try
            {
                this.context.Products.Add(product);

                var newInventory = new ProductInventory()
                {
                    Product = product,
                    QuantityOnHand = 0,
                    IdealQuantity = 10
                };

                this.context.ProductInventories.Add(newInventory);
                this.context.SaveChanges();

                return new ServiceResponse<Product>()
                {
                    Data = product,
                    Time = DateTime.UtcNow,
                    Message = "Saved new product",
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<Product>()
                {
                    Data = product,
                    Time = DateTime.UtcNow,
                    Message = $"Error saving new product: { e.InnerException }",
                    IsSuccess = false
                };
            }
        }

        public async Task<IList<Product>> GetAllProducts()
        {
            return await this.context.Products.ToListAsync();
        }

        /// <summary>
        /// Retrieves a product by primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product> GetProductById(int id)
        {
            return await this.context.Products.FindAsync(id);
        }
    }
}
