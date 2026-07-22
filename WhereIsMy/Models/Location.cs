namespace WhereIsMy;
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Привязка к юзеру
    public int UserId { get; set; }
    public User User { get; set; } 

    // Навигационное свойство: список вещей внутри этой локации
    public List<Item> Items { get; set; } = new();
}
