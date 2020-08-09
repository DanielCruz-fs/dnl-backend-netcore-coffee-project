using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Serialization
{
    public static class ProductMapper
    {
        public static ProductModel SerializationProductModel(Product product)
        {
            return new ProductModel()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsTaxable = product.IsTaxable,
                IsArchived = product.IsArchived,
                CreatedOn = product.CreatedOn,
                UpdatedOn = product.UpdatedOn
            };

        }

        public static Product SerializationProduct(ProductModel product)
        {
            return new Product()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsTaxable = product.IsTaxable,
                IsArchived = product.IsArchived,
                CreatedOn = product.CreatedOn,
                UpdatedOn = product.UpdatedOn
            };

        }
    }
}
