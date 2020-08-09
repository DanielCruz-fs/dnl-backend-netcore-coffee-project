using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Service.Contracts;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductController> logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetProducts()
        {
            var products = await this.productService.GetAllProducts();
            var productViewModels = products.Select(ProductMapper.SerializationProductModel);
            return Ok(productViewModels);
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductModel productModel)
        {
            // View Model validation in action
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            this.logger.LogInformation("Adding a product");
            var newProduct = ProductMapper.SerializationProduct(productModel);
            return Ok(this.productService.CreateProduct(newProduct));
        }

        [HttpPut("archive/{id}")]
        public IActionResult ArchiveProduct(int id)
        {
            this.logger.LogInformation("Archiving product");
            var archivedResult = this.productService.ArchiveProduct(id);
            return Ok(archivedResult);
        }
    }
}
