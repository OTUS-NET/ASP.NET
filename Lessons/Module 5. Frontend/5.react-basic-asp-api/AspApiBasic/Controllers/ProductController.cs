using AspApiBasic.Model;
using Microsoft.AspNetCore.Mvc;

namespace AspApiBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    // Пример данных для каталога товаров (в реальном приложении используйте базу данных)
    private static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Price = 50000 },
        new Product { Id = 2, Name = "Смартфон", Price = 30000 },
        new Product { Id = 3, Name = "Наушники", Price = 5000 }
    };

    // GET: api/product
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(Products);
    }

    // GET: api/product/{id}
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}