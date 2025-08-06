using Xunit;
using webapp1.Models;

namespace webapp1.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(0, product.Id);
            Assert.Equal(string.Empty, product.Name);
            Assert.Equal(0m, product.Price);
            Assert.Equal(string.Empty, product.Category);
            Assert.Equal(default(DateTime), product.CreatedDate);
            Assert.True(product.IsActive);
        }

        [Fact]
        public void Product_PropertyAssignment_SetsCorrectValues()
        {
            // Arrange
            var product = new Product();
            var expectedDate = DateTime.Now;

            // Act
            product.Id = 1;
            product.Name = "Test Product";
            product.Price = 99.99m;
            product.Category = "Electronics";
            product.CreatedDate = expectedDate;
            product.IsActive = false;

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(99.99m, product.Price);
            Assert.Equal("Electronics", product.Category);
            Assert.Equal(expectedDate, product.CreatedDate);
            Assert.False(product.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Product Name")]
        [InlineData("Very Long Product Name With Multiple Words")]
        public void Product_Name_AcceptsVariousStrings(string name)
        {
            // Arrange
            var product = new Product();

            // Act
            product.Name = name;

            // Assert
            Assert.Equal(name, product.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(999999.99)]
        public void Product_Price_AcceptsValidDecimalValues(decimal price)
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = price;

            // Assert
            Assert.Equal(price, product.Price);
        }

        [Theory]
        [InlineData("Electronics")]
        [InlineData("Books")]
        [InlineData("Clothing")]
        [InlineData("")]
        public void Product_Category_AcceptsVariousCategories(string category)
        {
            // Arrange
            var product = new Product();

            // Act
            product.Category = category;

            // Assert
            Assert.Equal(category, product.Category);
        }

        [Fact]
        public void Product_CreatedDate_CanBeSetToAnyDateTime()
        {
            // Arrange
            var product = new Product();
            var pastDate = DateTime.Now.AddYears(-1);
            var futureDate = DateTime.Now.AddYears(1);

            // Act & Assert - Past date
            product.CreatedDate = pastDate;
            Assert.Equal(pastDate, product.CreatedDate);

            // Act & Assert - Future date
            product.CreatedDate = futureDate;
            Assert.Equal(futureDate, product.CreatedDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Product_IsActive_CanBeSetToBothValues(bool isActive)
        {
            // Arrange
            var product = new Product();

            // Act
            product.IsActive = isActive;

            // Assert
            Assert.Equal(isActive, product.IsActive);
        }
    }
}