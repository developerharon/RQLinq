using RQLinq.UnitTests.Infrastructure;

namespace RQLinq.UnitTests
{
    internal static class TestData
    {
        public static List<Order> Orders => new List<Order>
        {
            new Order {
                Id = 1,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Apple", Price = 0.50m },
                    new Product { Id = 2, Name = "Banana", Price = 0.25m },
                    new Product { Id = 3, Name = "Orange", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Nakuru" } } },
                DescriptionTags = new List<string> { "fruits", "fresh" } },
            new Order {
                Id = 2,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Books", Price = 0.50m },
                    new Product { Id = 2, Name = "Pencils", Price = 0.25m },
                    new Product { Id = 3, Name = "Erasers", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Nairobi" } } },
                DescriptionTags = new List<string> { "school", "things" } },
            new Order {
                Id = 3,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Beef", Price = 0.50m },
                    new Product { Id = 2, Name = "Chicken", Price = 0.25m },
                    new Product { Id = 3, Name = "Pork", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Mombasa" } } },
                DescriptionTags = new List<string> { "meat", "animal" } },
            new Order {
                Id = 4,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Detol", Price = 0.50m },
                    new Product { Id = 2, Name = "Geisha", Price = 0.25m },
                    new Product { Id = 3, Name = "Menengai", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Kisumu" } } },
                DescriptionTags = new List<string> { "bathing", "soap", "school" } },
        };
    }
}