using System;
using CRUDwithMinimalAPI;
using CRUDwithMinimalAPI.Models;

namespace MyMinimalApi.Tests
{
    public static class TestDataSeeder
    {
        public static void SeedTestData(AppDbContext dbContext)
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = (decimal)10.99 },
                new Product { Id = 2, Name = "Product 2", Price = (decimal)20.99 },
            };
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();
        }
    }
}

