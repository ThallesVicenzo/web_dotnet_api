public static class ProductRepository
{
  public static List<Product>? Products { get; set; } = Products = [];

  public static void Init(IConfiguration configuration)
  {
    var products = configuration.GetSection("Products").Get<List<Product>>();
    Products = products;
  }


  public static void add(Product product)
  {
    if (Products == null)
      Products?.Add(product);
  }

  public static Product? getBy(string code)
  {
    return Products?.FirstOrDefault(p => p.Code == code);
  }

  public static void remove(Product product)
  {
    Products?.Remove(product);
  }


}
