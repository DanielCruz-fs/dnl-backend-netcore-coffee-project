using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Service.Contracts
{
    public interface IProductService
    {
        Task<IList<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        ServiceResponse<Product> CreateProduct(Product product);
        ServiceResponse<Product> ArchiveProduct(int id);
    }
}
