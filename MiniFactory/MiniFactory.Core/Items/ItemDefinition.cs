using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ItemDefinition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Weight { get; set; }
    public float Price { get; set; }
    public Color ItemColor { get; set; }
    public Texture2D ItemTexture { get; set; }

    public ItemDefinition(string name, string description, float weight, float price, Color color, Texture2D texture)
    {
        Name = name;
        Description = description;
        Weight = weight;
        Price = price;
        ItemColor = color;
        ItemTexture = texture;
    }
}