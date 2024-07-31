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

app.Run();

public class Product
{
  public string Code { get; set; }
  public string Name { get; set; }
}
