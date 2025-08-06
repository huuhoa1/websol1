using Xunit;
using Microsoft.EntityFrameworkCore;
using webapp1.Data;
using webapp1.Models;

namespace webapp1.Tests.Data
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            
            // Clear any seed data that might exist
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();
        }

        [Fact]
        public void Context_HasProductsDbSet()
        {
            // Assert
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void Context_CanAddAndSaveProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Category = "Test Category",
                Price = 99.99m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            // Act
            _context.Products.Add(product);
            var changes = _context.SaveChanges();

            // Assert
            Assert.Equal(1, changes);
            Assert.True(product.Id > 0); // Id should be set after save
        }

        [Fact]
        public async Task Context_CanQueryProducts()
        {
            // Arrange
            var testProduct = new Product
            {
                Name = "Query Test Product",
                Category = "Test",
                Price = 50.00m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Products.Add(testProduct);
            await _context.SaveChangesAsync();

            // Act
            var retrievedProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == "Query Test Product");

            // Assert
            Assert.NotNull(retrievedProduct);
            Assert.Equal("Query Test Product", retrievedProduct.Name);
            Assert.Equal("Test", retrievedProduct.Category);
            Assert.Equal(50.00m, retrievedProduct.Price);
        }

        [Fact]
        public async Task Context_CanUpdateProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Original Name",
                Category = "Original Category",
                Price = 100.00m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated Name";
            product.Price = 200.00m;
            await _context.SaveChangesAsync();

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Name", updatedProduct.Name);
            Assert.Equal(200.00m, updatedProduct.Price);
            Assert.Equal("Original Category", updatedProduct.Category); // Should remain unchanged
        }

        [Fact]
        public async Task Context_CanDeleteProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "To Be Deleted",
                Category = "Test",
                Price = 75.00m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var productId = product.Id;

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var deletedProduct = await _context.Products.FindAsync(productId);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task Context_CanFilterActiveProducts()
        {
            // Arrange
            var activeProduct = new Product
            {
                Name = "Active Product",
                Category = "Test",
                Price = 100.00m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            var inactiveProduct = new Product
            {
                Name = "Inactive Product",
                Category = "Test",
                Price = 50.00m,
                CreatedDate = DateTime.Now,
                IsActive = false
            };

            _context.Products.AddRange(activeProduct, inactiveProduct);
            await _context.SaveChangesAsync();

            // Act
            var activeProducts = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            // Assert
            Assert.Single(activeProducts);
            Assert.Equal("Active Product", activeProducts[0].Name);
        }

        [Fact]
        public async Task Context_CanOrderProductsByName()
        {
            // Arrange
            var products = new[]
            {
                new Product { Name = "Zebra Product", Category = "Test", Price = 10m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Alpha Product", Category = "Test", Price = 20m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Beta Product", Category = "Test", Price = 30m, CreatedDate = DateTime.Now, IsActive = true }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var orderedProducts = await _context.Products
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToListAsync();

            // Assert
            Assert.Equal(new[] { "Alpha Product", "Beta Product", "Zebra Product" }, orderedProducts);
        }

        [Fact]
        public async Task Context_CanFilterByCategory()
        {
            // Arrange
            var electronicsProducts = new[]
            {
                new Product { Name = "Laptop", Category = "Electronics", Price = 1000m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Mouse", Category = "Electronics", Price = 30m, CreatedDate = DateTime.Now, IsActive = true }
            };

            var furnitureProduct = new Product 
            { 
                Name = "Chair", 
                Category = "Furniture", 
                Price = 150m, 
                CreatedDate = DateTime.Now, 
                IsActive = true 
            };

            _context.Products.AddRange(electronicsProducts);
            _context.Products.Add(furnitureProduct);
            await _context.SaveChangesAsync();

            // Act
            var electronics = await _context.Products
                .Where(p => p.Category == "Electronics")
                .ToListAsync();

            // Assert
            Assert.Equal(2, electronics.Count);
            Assert.All(electronics, p => Assert.Equal("Electronics", p.Category));
        }

        [Fact]
        public async Task Context_CanFilterByPriceRange()
        {
            // Arrange
            var products = new[]
            {
                new Product { Name = "Cheap Item", Category = "Test", Price = 10m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Mid Range Item", Category = "Test", Price = 100m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Expensive Item", Category = "Test", Price = 1000m, CreatedDate = DateTime.Now, IsActive = true }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var midRangeProducts = await _context.Products
                .Where(p => p.Price >= 50m && p.Price <= 500m)
                .ToListAsync();

            // Assert
            Assert.Single(midRangeProducts);
            Assert.Equal("Mid Range Item", midRangeProducts[0].Name);
        }

        [Fact]
        public async Task Context_CanCountProducts()
        {
            // Arrange
            var products = new[]
            {
                new Product { Name = "Product 1", Category = "Test", Price = 10m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Product 2", Category = "Test", Price = 20m, CreatedDate = DateTime.Now, IsActive = true },
                new Product { Name = "Product 3", Category = "Test", Price = 30m, CreatedDate = DateTime.Now, IsActive = false }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var totalCount = await _context.Products.CountAsync();
            var activeCount = await _context.Products.CountAsync(p => p.IsActive);

            // Assert
            Assert.Equal(3, totalCount);
            Assert.Equal(2, activeCount);
        }

        [Fact]
        public async Task Context_CanCheckIfProductExists()
        {
            // Arrange
            var product = new Product
            {
                Name = "Existence Test",
                Category = "Test",
                Price = 25m,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var exists = await _context.Products.AnyAsync(p => p.Name == "Existence Test");
            var notExists = await _context.Products.AnyAsync(p => p.Name == "Non-Existent Product");

            // Assert
            Assert.True(exists);
            Assert.False(notExists);
        }

        [Fact]
        public void Context_HasCorrectConnectionType()
        {
            // Act & Assert
            Assert.IsType<ApplicationDbContext>(_context);
            Assert.NotNull(_context.Database);
        }

        [Fact]
        public async Task Context_CanHandleEmptyDatabase()
        {
            // Act
            var products = await _context.Products.ToListAsync();
            var count = await _context.Products.CountAsync();

            // Assert
            Assert.Empty(products);
            Assert.Equal(0, count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}