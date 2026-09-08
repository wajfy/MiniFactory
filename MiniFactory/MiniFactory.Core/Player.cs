using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Player
{
    public Vector2 Position { get; private set; }

    private readonly Texture2D _texture;
    private readonly float _speed = 200f;
    public int MiningRange = 200;
    public float MiningSpeed = 3f;
    public Inventory Inventory;
    private const int Size = 32;

    private bool inRange { get; set; }
    private ResourceTile currentResourceTile { get; set; }
    private float miningProgress { get; set; }

    //progress bar
    private const int barWidth = 60;
    private const int barHeight = 10;
    private const int barVerticalOffset = 20;

    //border
    const int borderThickness = 4;

    public Player(Vector2 startPosition, Texture2D texture)
    {
        Position = startPosition;
        _texture = texture;
        Inventory = new Inventory(9, 3);
    }

    public void Update(GameTime gameTime, World world, MouseState mouse, Vector2 mousePosition)
    {
        UpdatePlayerMovement(gameTime, world);
        MineResource(gameTime, world, mouse, mousePosition);
    }

    public void DrawWorld(SpriteBatch spriteBatch, Point mouseGrid, UITheme theme)
    {
        DrawResourceBorder(spriteBatch, mouseGrid, theme);
        DrawCharacter(spriteBatch);
    }

    public void DrawScreen(SpriteBatch spriteBatch, Vector2 mousePosition, Point mouseGrid, UITheme theme)
    {
        DrawMiningUI(spriteBatch, mousePosition, mouseGrid, theme);
    }

    private void UpdatePlayerMovement(GameTime gameTime, World world)
    {
        KeyboardState keyboard = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S))
            direction.Y += 1;
        if (keyboard.IsKeyDown(Keys.A))
            direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.D))
            direction.X += 1;

        Vector2 movement = direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float newX = Position.X + movement.X;
       
        Rectangle rectAtNewX = new Rectangle((int)newX, (int)Position.Y, Size, Size);
        if (!OverlapsWater(world, rectAtNewX))
            Position = new Vector2(newX, Position.Y);

        float newY = Position.Y + movement.Y;
        Rectangle rectAtNewY = new Rectangle((int)Position.X, (int)newY, Size, Size);
        if (!OverlapsWater(world, rectAtNewY))
            Position = new Vector2(Position.X, newY);
    }
    private bool OverlapsWater(World world, Rectangle rect)
    {
        Point topLeft = World.PixelToGrid(new Vector2(rect.Left, rect.Top));
        Point bottomRight = World.PixelToGrid(new Vector2(rect.Right - 1, rect.Bottom - 1));

        for (int i = topLeft.X; i <= bottomRight.X; i++)
        {
            for (int j = topLeft.Y; j <= bottomRight.Y; j++)
            {
                if(world.IsWater(i, j))
                    return true;
            }
        }
        return false;
    }
    private void MineResource(GameTime gameTime, World world, MouseState mouse, Vector2 mousePosition)
    {
        Point mouseGrid = World.PixelToGrid(mousePosition);
        world.TryGetResourceAt(new Vector2(mousePosition.X, mousePosition.Y), out ResourceTile tile);
        if (tile != currentResourceTile)
            miningProgress = 0;
        currentResourceTile = tile;

        if (currentResourceTile != null)
        {
            Vector2 current = World.GridToPixel(mouseGrid);
            Vector2 tileCenter = new Vector2(current.X + (World.TileSize / 2), current.Y + (World.TileSize / 2));
            inRange = Vector2.Distance(Position, tileCenter) <= MiningRange;

            if (mouse.LeftButton == ButtonState.Pressed && inRange)
            {
                miningProgress += MiningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (miningProgress >= currentResourceTile.MiningDuration)
                {
                    miningProgress = 0;
                    currentResourceTile.Remaining -= 1;
                    Inventory.TryAddItem(currentResourceTile.Type);
                    world.RemoveIfDepleted(mouseGrid);
                }
            }
            else
            {
                miningProgress = 0;
            }
        }
        else
        {
            inRange = false;
        }
    }

    private void DrawCharacter(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, Size, Size), Color.White);
    }

    private void DrawResourceBorder(SpriteBatch spriteBatch, Point mouseGrid, UITheme theme)
    {
        if (inRange)
        {
            Vector2 corner = World.GridToPixel(mouseGrid);
            spriteBatch.DrawHollowRect(theme.Pixel, corner, theme.Accent, borderThickness);
        }
    }
    private void DrawMiningUI(SpriteBatch spriteBatch, Vector2 mousePosition, Point mouseGrid, UITheme theme)
    {
        //progress
        if (miningProgress > 0)
        {
            spriteBatch.Draw(theme.Pixel, new Rectangle((int)mousePosition.X - (barWidth / 2) - 4, (int)mousePosition.Y - barVerticalOffset - 4, barWidth + 8, barHeight + 8), Color.Black);
            spriteBatch.Draw(theme.Pixel, new Rectangle((int)mousePosition.X - (barWidth / 2), (int)mousePosition.Y - barVerticalOffset, (int)(barWidth * (miningProgress / currentResourceTile.MiningDuration)), barHeight), Color.LimeGreen);
        }

        //hud + range hollow
        if (currentResourceTile != null)
        {
            spriteBatch.DrawString(theme.FontMedium, currentResourceTile.Type.ToString(), new Vector2(10, 10), theme.TextColor);
            spriteBatch.DrawString(theme.FontMedium, currentResourceTile.Remaining.ToString(), new Vector2(10, 60), theme.TextColor);
        }
    }
}