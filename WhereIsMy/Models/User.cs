namespace WhereIsMy;
public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;      // красивое имя
    public string Login { get; set; } = null!;     // по нему логинимся
    public string PasswordHash { get; set; } = null!;

    public List<Location> Locations { get; set; } = new();
    
    // Также юзер владеет и своими вещами
    public List<Item> Items { get; set; } = new();
}
