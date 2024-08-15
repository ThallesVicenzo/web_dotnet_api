public class Product
{
  public int Id { get; set; }
  public required string Code { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public required Category Category { get; set; }
  public List<Tag?>? Tags { get; set; }
}
