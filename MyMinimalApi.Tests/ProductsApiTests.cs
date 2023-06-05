//using System;
//using CRUDwithMinimalAPI;
//using CRUDwithMinimalAPI.Models;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using System.Net;
//using System.Net.Http.Json;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.EntityFrameworkCore;

//namespace MinimalApiCrud.Tests
//{
//    public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
//    {
//        private readonly WebApplicationFactory<Program> _factory;

//        public ProductsApiTests(WebApplicationFactory<Program> factory)
//        {
//            _factory = factory;
//        }

//        [Fact]
//        public async Task GetAllProducts_ReturnsOkResponse()
//        {
//            var client = _factory.CreateClient();
//            var response = await client.GetAsync("/products");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetProductById_ExistingId_ReturnsOkResponse()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var serviceProvider = scope.ServiceProvider;
//            var client = _factory.CreateClient();
//            using var dbContextScope = serviceProvider.CreateScope();
//            var dbContext = dbContextScope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var product = new Product
//            {
//                Name = "Test Product",
//                Price = (decimal)10.99
//            };
//            dbContext.Products.Add(product);
//            dbContext.SaveChanges();
//            var response = await client.GetAsync($"/products/{product.Id}");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            dbContext.Products.Remove(product);
//            dbContext.SaveChanges();
//        }

//        [Fact]
//        public async Task GetProductById_NonExistingId_ReturnsNotFoundResponse()
//        {
//            var client = _factory.CreateClient();
//            var response = await client.GetAsync("/products/999");
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreateProduct_ValidData_ReturnsCreatedResponse()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var serviceProvider = scope.ServiceProvider;
//            var client = _factory.CreateClient();
//            using var dbContextScope = serviceProvider.CreateScope();
//            var dbContext = dbContextScope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var product = new Product
//            {
//                Name = "Test Product",
//                Price = (decimal)10.99
//            };
//            var response = await client.PostAsJsonAsync("/products", product);
//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//            var createdProduct = dbContext.Products.FirstOrDefault(p => p.Name == "Test Product");
//            if (createdProduct != null)
//            {
//                dbContext.Products.Remove(createdProduct);
//                dbContext.SaveChanges();
//            }
//        }

//        [Fact]
//        public async Task UpdateProduct_ExistingId_ReturnsNoContentResponse()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var serviceProvider = scope.ServiceProvider;
//            var client = _factory.CreateClient();
//            using var dbContextScope = serviceProvider.CreateScope();
//            var dbContext = dbContextScope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var product = new Product
//            {
//                Name = "Test Product",
//                Price = (decimal)10.99
//            };
//            dbContext.Products.Add(product);
//            dbContext.SaveChanges();
//            product.Name = "Updated Product";
//            product.Price = (decimal)19.99;
//            var response = await client.PutAsJsonAsync($"/products/{product.Id}", product);
//            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//            dbContext.Products.Remove(product);
//            dbContext.SaveChanges();
//        }

//        [Fact]
//        public async Task UpdateProduct_NonExistingId_ReturnsNotFoundResponse()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var serviceProvider = scope.ServiceProvider;
//            var client = _factory.CreateClient();
//            using var dbContextScope = serviceProvider.CreateScope();
//            var dbContext = dbContextScope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var product = new Product
//            {
//                Id = 999,
//                Name = "Test Product",
//                Price = (decimal)10.99
//            };
//            var response = await client.PutAsJsonAsync($"/products/{product.Id}", product);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task DeleteProduct_ExistingId_ReturnsNoContentResponse()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var serviceProvider = scope.ServiceProvider;
//            var client = _factory.CreateClient();
//            using var dbContextScope = serviceProvider.CreateScope();
//            var dbContext = dbContextScope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var product = new Product
//            {
//                Name = "Test Product",
//                Price = (decimal)25.99
//            };
//            dbContext.Products.Add(product);
//            dbContext.SaveChanges();
//            var existingProduct = await dbContext.Products.FindAsync(product.Id);
//            Assert.NotNull(existingProduct);
//            var response = await client.DeleteAsync($"/products/{product.Id}");
//            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//            dbContext.SaveChanges();
//        }

//        [Fact]
//        public async Task DeleteProduct_NonExistingId_ReturnsNotFoundResponse()
//        {
//            var client = _factory.CreateClient();
//            var response = await client.DeleteAsync("/products/999");
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }
//    }
//}

using System;
using CRUDwithMinimalAPI;
using CRUDwithMinimalAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MinimalApiCrud.Tests
{
    public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClient()
        {
            return _factory.CreateClient();
        }

        private IServiceScope CreateScope()
        {
            var scope = _factory.Services.CreateScope();
            return scope;
        }

        private AppDbContext GetDbContext(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return dbContext;
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResponse()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/products");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_ExistingId_ReturnsOkResponse()
        {
            using var scope = CreateScope();
            var dbContext = GetDbContext(scope);

            var client = CreateClient();
            var product = new Product
            {
                Name = "Test Product",
                Price = (decimal)10.99
            };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            var response = await client.GetAsync($"/products/{product.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetProductById_NonExistingId_ReturnsNotFoundResponse()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/products/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ValidData_ReturnsCreatedResponse()
        {
            using var scope = CreateScope();
            var dbContext = GetDbContext(scope);

            var client = CreateClient();
            var product = new Product
            {
                Name = "Test Product",
                Price = (decimal)10.99
            };

            var response = await client.PostAsJsonAsync("/products", product);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
            if (createdProduct != null)
            {
                dbContext.Products.Remove(createdProduct);
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task UpdateProduct_ExistingId_ReturnsNoContentResponse()
        {
            using var scope = CreateScope();
            var dbContext = GetDbContext(scope);

            var client = CreateClient();
            var product = new Product
            {
                Name = "Test Product",
                Price = (decimal)10.99
            };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            product.Name = "Updated Product";
            product.Price = (decimal)19.99;

            var response = await client.PutAsJsonAsync($"/products/{product.Id}", product);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task UpdateProduct_NonExistingId_ReturnsNotFoundResponse()
        {
            using var scope = CreateScope();
            var client = CreateClient();

            var response = await client.PutAsJsonAsync("/products/999", new Product());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ExistingId_ReturnsNoContentResponse()
        {
            using var scope = CreateScope();
            var dbContext = GetDbContext(scope);

            var client = CreateClient();
            var product = new Product
            {
                Name = "Test Product",
                Price = (decimal)25.99
            };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            var existingProduct = await dbContext.Products.FindAsync(product.Id);
            Assert.NotNull(existingProduct);

            var response = await client.DeleteAsync($"/products/{product.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task DeleteProduct_NonExistingId_ReturnsNotFoundResponse()
        {
            var client = CreateClient();
            var response = await client.DeleteAsync("/products/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
