public class Order
{
    public int Id { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<string> DescriptionTags { get; set; }
    public IDictionary<string, string[]> Tags { get; set; }
}