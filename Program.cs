
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

var configuration = app.Configuration;

ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World!");
app.MapGet("/user", () => new { name = "Thalles Vicenzo", age = 22 });
app.MapGet("/AddHeader", (HttpResponse response) =>
{
  response.Headers.Append("Test", "Just testing");
  return "Testing";
});

app.MapPost("/products", (Product product) =>
{
  ProductRepository.add(product);
  return Results.Created($"/product/{product.Code}", product.Code);
});

//required params per URL
app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
  var product = ProductRepository.getBy(code);
  if (product != null)
    return Results.Ok(product);

  return Results.NotFound();
});

app.MapPut("/products", (Product product) =>
{
  var productSaved = ProductRepository.getBy(product.Code);

  productSaved!.Name = product.Name;

  return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
  var productSaved = ProductRepository.getBy(code);
  ProductRepository.remove(productSaved!);

  return Results.Ok();
});

//dynamic params per URL
// app.MapGet("/products", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
// {
//   return dateStart + " - " + dateEnd;
// });

// app.MapGet("/getproductWithHeader", (HttpRequest request) =>
// {
//   return request.Headers["product-code"].ToString();
// }
// );
if (app.Environment.IsStaging())
  app.MapGet("/configuration/database", (IConfiguration configuration) =>
  {
    return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
  });

app.Run();

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

public class Category
{
  public int Id { get; set; }
  public required string Name { get; set; }
}

public class Tag
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public int ProductId { get; set; }
}

public class Product
{
  public int Id { get; set; }
  public required string Code { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public required Category Category { get; set; }
  public required List<Tag> Tags { get; set; }
}

public class ApplicationDbContext : DbContext
{
  public DbSet<Product> Products { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(500).IsRequired(false);
    modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
    modelBuilder.Entity<Product>().Property(p => p.Code).HasMaxLength(20).IsRequired();
  }

  protected override void OnConfiguring(DbContextOptionsBuilder options)
  => options.UseSqlServer(
      "Server=localhost;Database=Products;User Id=sa;Password=@Sql2022;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
}
