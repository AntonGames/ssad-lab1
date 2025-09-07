using ProductManager.Models;

namespace ProductManager.Services
{
    public class ProductService : IProductService
    {
        // In-memory storage (no database required)
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop for professionals", Price = 999.99m, Quantity = 10 },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless ergonomic mouse with precision tracking", Price = 29.99m, Quantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard with RGB backlighting", Price = 89.99m, Quantity = 25 }
        };

        private static int _nextId = 4;

        public List<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Quantity = product.Quantity;
            }
        }

        public void Delete(int id)
        {
            _products.RemoveAll(p => p.Id == id);
        }
    }
}