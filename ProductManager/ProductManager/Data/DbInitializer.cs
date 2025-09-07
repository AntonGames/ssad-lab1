using ProductManager.Models;

namespace ProductManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if Products table has data
            if (context.Products.Any())
            {
                return; // DB has been seeded
            }

            // Seed initial data
            var products = new Product[]
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for professionals",
                    Price = 999.99m,
                    Quantity = 10
                },
                new Product
                {
                    Name = "Mouse",
                    Description = "Wireless ergonomic mouse with precision tracking",
                    Price = 29.99m,
                    Quantity = 50
                },
                new Product
                {
                    Name = "Keyboard",
                    Description = "Mechanical keyboard with RGB backlighting",
                    Price = 89.99m,
                    Quantity = 25
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}