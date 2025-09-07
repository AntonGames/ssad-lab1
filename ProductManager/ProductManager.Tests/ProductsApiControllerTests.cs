using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManager.Controllers;
using ProductManager.Models;
using ProductManager.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductManager.Tests
{
    public class ProductsApiControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsApiController _controller;

        public ProductsApiControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsApiController(_mockService.Object);
        }

        // GET Tests
        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product 1", Description = "Test Description 1", Price = 10.99m, Quantity = 5 },
                new Product { Id = 2, Name = "Test Product 2", Description = "Test Description 2", Price = 20.99m, Quantity = 10 }
            };
            _mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, ((List<Product>)returnedProducts).Count);
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithEmptyList_WhenNoProducts()
        {
            // Arrange
            var products = new List<Product>();
            _mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Empty((List<Product>)returnedProducts);
        }

        [Fact]
        public async Task GetProduct_ExistingId_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Description = "Test Description", Price = 10.99m, Quantity = 5 };
            _mockService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
            Assert.Equal("Test Product", returnedProduct.Name);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetByIdAsync(999)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        // POST Tests
        [Fact]
        public async Task PostProduct_ValidProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 15.99m, Quantity = 8 };
            var createdProduct = new Product { Id = 3, Name = "New Product", Description = "New Description", Price = 15.99m, Quantity = 8 };
            _mockService.Setup(service => service.AddAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.PostProduct(product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
            var returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(3, returnedProduct.Id);
            Assert.Equal("New Product", returnedProduct.Name);
            _mockService.Verify(s => s.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task PostProduct_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var product = new Product { Description = "Missing Name", Price = 10m, Quantity = 5 };

            // Act
            var result = await _controller.PostProduct(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _mockService.Verify(s => s.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task PostProduct_NullProduct_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("", "Product is required");

            // Act
            var result = await _controller.PostProduct(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // PUT Tests
        [Fact]
        public async Task PutProduct_ValidProduct_ReturnsNoContent()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Product", Description = "Updated Description", Price = 25.99m, Quantity = 15 };
            _mockService.Setup(service => service.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(service => service.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutProduct(1, product);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task PutProduct_MismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Id = 2, Name = "Product", Description = "Description", Price = 10m, Quantity = 5 };

            // Act
            var result = await _controller.PutProduct(1, product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task PutProduct_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "", Description = "Description", Price = 10m, Quantity = 5 };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.PutProduct(1, product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task PutProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var product = new Product { Id = 999, Name = "Product", Description = "Description", Price = 10m, Quantity = 5 };
            _mockService.Setup(service => service.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutProduct(999, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockService.Verify(s => s.ExistsAsync(999), Times.Once);
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        // DELETE Tests
        [Fact]
        public async Task DeleteProduct_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(service => service.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockService.Verify(s => s.ExistsAsync(999), Times.Once);
            _mockService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProduct_ValidId_CallsServiceOnce()
        {
            // Arrange
            _mockService.Setup(service => service.ExistsAsync(5)).ReturnsAsync(true);
            _mockService.Setup(service => service.DeleteAsync(5)).Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteProduct(5);

            // Assert
            _mockService.Verify(s => s.DeleteAsync(5), Times.Once);
        }
    }
}