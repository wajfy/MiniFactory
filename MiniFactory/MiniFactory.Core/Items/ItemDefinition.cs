using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ItemDefinition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Weight { get; set; }
    public float Price { get; set; }
    public Rectangle ItemSourceRect { get; set; }

    public ItemDefinition(string name, string description, float weight, float price, Rectangle sourceRect)
    {
        Name = name;
        Description = description;
        Weight = weight;
        Price = price;
        ItemSourceRect = sourceRect;
    }
}