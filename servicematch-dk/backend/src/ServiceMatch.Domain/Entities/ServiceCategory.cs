namespace ServiceMatch.Domain.Entities;

public class ServiceCategory
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    private ServiceCategory() { }

    public ServiceCategory(int id, string name, string slug)
    {
        Id = id;
        Name = name;
        Slug = slug;
    }
}
