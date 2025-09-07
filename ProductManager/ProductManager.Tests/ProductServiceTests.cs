using Moq;
using ProductManager.Models;
using ProductManager.Repositories;
using ProductManager.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductManager.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10m, Quantity = 5 },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20m, Quantity = 10 }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Product 1", result[0].Name);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product", Description = "Description", Price = 10m, Quantity = 5 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Product", result.Name);
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ValidProduct_ReturnsAddedProduct()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 15m, Quantity = 8 };
            var addedProduct = new Product { Id = 3, Name = "New Product", Description = "New Description", Price = 15m, Quantity = 8 };
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>())).ReturnsAsync(addedProduct);

            // Act
            var result = await _service.AddAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("New Product", result.Name);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepository()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated", Description = "Updated Desc", Price = 25m, Quantity = 15 };
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(product);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepository()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.ExistsAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.ExistsAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.ExistsAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.ExistsAsync(999), Times.Once);
        }
    }
}