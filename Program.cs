
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

  productSaved.Name = product.Name;

  return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
  var productSaved = ProductRepository.getBy(code);
  ProductRepository.remove(productSaved);

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

app.Run();

public static class ProductRepository
{
  public static List<Product>? Products { get; set; }

  public static void add(Product product)
  {
    if (Products == null)
      Products = new List<Product>();

    Products.Add(product);
  }

  public static Product getBy(string code)
  {
    return Products.FirstOrDefault(p => p.Code == code);
  }

  public static void remove(Product product)
  {
    Products.Remove(product);
  }


}

public class Product
{
  public required string Code { get; set; }
  public required string Name { get; set; }
}
