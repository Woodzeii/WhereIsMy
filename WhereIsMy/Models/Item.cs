public class Item
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public  string Description { get; set; }
    public required string Location { get; set; }
    public DateTime DateAdded { get; set; }
}