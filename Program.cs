
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

app.MapPost("/saveproduct", (Product product) =>
{
  return product.Code + " - " + product.Name;
});

//dynamic params per URL
app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
  return dateStart + " - " + dateEnd;
});

//required params per URL
app.MapGet("/getproduct/{code}", ([FromRoute] string code) =>
{
  return code;
});

app.MapGet("/getproductWithHeader", (HttpRequest request) =>
{
  return request.Headers["product-code"].ToString();
}
);

app.Run();

public class Product
{
  public required string Code { get; set; }
  public required string Name { get; set; }
}
