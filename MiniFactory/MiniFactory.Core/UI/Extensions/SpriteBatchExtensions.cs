using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class SpriteBatchExtensions
{
    public static void DrawRoundedRect(this SpriteBatch spriteBatch, Texture2D pixel, Texture2D cornerTexture, Rectangle destination, Color color, int cornerSize = 8)
    {
        Rectangle topLeftSource = new Rectangle(0, 0, cornerSize, cornerSize);
        Rectangle topRightSource = new Rectangle(32 - cornerSize, 0, cornerSize, cornerSize);
        Rectangle bottomLeftSource = new Rectangle(0, 32 - cornerSize, cornerSize, cornerSize);
        Rectangle bottomRightSource = new Rectangle(32 - cornerSize, 32 - cornerSize, cornerSize, cornerSize);

        Rectangle topLeftDestination = new Rectangle(destination.X, destination.Y, cornerSize, cornerSize);
        Rectangle topRightDestination = new Rectangle(destination.X + destination.Width - cornerSize, destination.Y, cornerSize, cornerSize);
        Rectangle bottomLeftDestination = new Rectangle(destination.X, destination.Y + destination.Height - cornerSize, cornerSize, cornerSize);
        Rectangle bottomRightDestination = new Rectangle(destination.X + destination.Width - cornerSize, destination.Y + destination.Height  - cornerSize, cornerSize, cornerSize);

        Rectangle verticalStrip = new Rectangle(destination.X, destination.Y + cornerSize, destination.Width, destination.Height  - 2 * cornerSize);
        Rectangle horizontalStrip = new Rectangle(destination.X + cornerSize, destination.Y, destination.Width - 2 * cornerSize, destination.Height);
        
        spriteBatch.Draw(pixel, verticalStrip, color);
        spriteBatch.Draw(pixel, horizontalStrip, color);

        spriteBatch.Draw(cornerTexture, topLeftDestination, topLeftSource, color); 
        spriteBatch.Draw(cornerTexture, topRightDestination, topRightSource, color); 
        spriteBatch.Draw(cornerTexture, bottomLeftDestination, bottomLeftSource, color); 
        spriteBatch.Draw(cornerTexture, bottomRightDestination, bottomRightSource, color); 
    }

    public static void DrawHollowRect(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 corner, Color color, int borderThickness)
    {
        // horní hrana
        spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y, World.TileSize, borderThickness), color);
        // dolní hrana
        spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y + World.TileSize - borderThickness, World.TileSize, borderThickness), color);
        // levá hrana
        spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y, borderThickness, World.TileSize), color);
        // pravá hrana
        spriteBatch.Draw(pixel, new Rectangle((int)corner.X + World.TileSize - borderThickness, (int)corner.Y, borderThickness, World.TileSize), color);
    }
}