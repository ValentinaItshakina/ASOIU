using Microsoft.EntityFrameworkCore;

namespace StoreApp
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=store.db");
        }

        public void SeedInitialData()
        {
            Database.EnsureCreated();
            if (Categories.Any()) return;

            var categories = new List<Category>
            {
                new Category { Name = "Электроника" },
                new Category { Name = "Одежда" },
                new Category { Name = "Книги" }
            };
            Categories.AddRange(categories);
            SaveChanges();

            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 50000, CategoryId = 1 },
                new Product { Name = "Смартфон", Price = 30000, CategoryId = 1 },
                new Product { Name = "Наушники", Price = 5000, CategoryId = 1 },
                new Product { Name = "Футболка", Price = 1500, CategoryId = 2 },
                new Product { Name = "Джинсы", Price = 3500, CategoryId = 2 },
                new Product { Name = "Куртка", Price = 8000, CategoryId = 2 },
                new Product { Name = "Роман", Price = 800, CategoryId = 3 },
                new Product { Name = "Детектив", Price = 650, CategoryId = 3 },
                new Product { Name = "Фантастика", Price = 900, CategoryId = 3 },
                new Product { Name = "Планшет", Price = 25000, CategoryId = 1 },
                new Product { Name = "Свитер", Price = 4000, CategoryId = 2 },
                new Product { Name = "Энциклопедия", Price = 2000, CategoryId = 3 }
            };
            Products.AddRange(products);
            SaveChanges();
        }
    }
}