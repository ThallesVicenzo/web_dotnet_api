
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

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

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) =>
{
  var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();

  var product = new Product
  {
    Code = productRequest.Code,
    Name = productRequest.Name,
    Description = productRequest.Description,
    Category = category,
  };

  if (productRequest.Tags != null)
  {
    product.Tags = new List<Tag?>();
    foreach (var item in productRequest.Tags)
    {
      product.Tags.Add(new Tag { Name = item });
    }
  }

  context.Products.Add(product);
  context.SaveChanges();

  return Results.Created($"/product/{product.Id}", product.Id);
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
