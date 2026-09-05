using Microsoft.Xna.Framework;

public class ItemDefinition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Weight { get; set; }
    public float Price { get; set; }
    public Color ItemColor { get; set; }

    public ItemDefinition(string name, string description, float weight, float price, Color color)
    {
        Name = name;
        Description = description;
        Weight = weight;
        Price = price;
        ItemColor = color;
    }
}