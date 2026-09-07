using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

public class World
{
    public const int TileSize = 64;

    // Konvence bitů sousedů (viz BlobTileset): N=1, NE=2, E=4, SE=8, S=16, SW=32, W=64, NW=128.
    private const int BitN = 1, BitNE = 2, BitE = 4, BitSE = 8, BitS = 16, BitSW = 32, BitW = 64, BitNW = 128;

    public TerrainType[,] Terrain;
    public bool[,] Water;
    public Dictionary<Point, Building> Buildings { get; set; }
    public Dictionary<Point, ResourceTile> Resources { get; set; }

    private BlobTileset _grassTileset;
    private Texture2D _water;

    public World(int mapWidth, int mapHeight)
    {
        Buildings = new Dictionary<Point, Building>();
        Resources = new Dictionary<Point, ResourceTile>();

        Terrain = new TerrainType[mapWidth, mapHeight];
        Water = new bool[mapWidth, mapHeight];

        for (int width = 0; width < mapWidth; width++)
            for (int height = 0; height < mapHeight; height++)
                Terrain[width, height] = TerrainType.Grass;
    }

    public void LoadContent(ContentManager content)
    {
        _grassTileset = new BlobTileset(content.Load<Texture2D>("Grass"));
        _water = content.Load<Texture2D>("Water");
    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Rectangle visibleGridBounds)
    {
        DrawTerrain(spriteBatch, visibleGridBounds);

        foreach (var resource in Resources)
        {
            resource.Value.Draw(spriteBatch, GridToPixel(resource.Key), texture);
        }
    }

    private void DrawTerrain(SpriteBatch spriteBatch, Rectangle visibleGridBounds)
    {
        int mapWidth = Terrain.GetLength(0);
        int mapHeight = Terrain.GetLength(1);

        for (int x = visibleGridBounds.Left; x < visibleGridBounds.Right; x++)
        {
            for (int y = visibleGridBounds.Top; y < visibleGridBounds.Bottom; y++)
            {
                Vector2 pixelPos = GridToPixel(new Point(x, y));
                Rectangle destRect = new Rectangle((int)pixelPos.X, (int)pixelPos.Y, TileSize, TileSize);

                // Voda je vždy podklad - i pod (průhlednými okraji) trávy.
                spriteBatch.Draw(_water, destRect, Color.White);

                bool inBounds = x >= 0 && y >= 0 && x < mapWidth && y < mapHeight;
                if (inBounds && !IsWater(x, y))
                {
                    int bitmask = ComputeBitmask(x, y);
                    Rectangle sourceRect = _grassTileset.GetSourceRect(bitmask);
                    spriteBatch.Draw(_grassTileset.Texture, destRect, sourceRect, Color.White);
                }
            }
        }
    }

    // Spočítá bitmasku 8 sousedů buňky (x, y) podle toho, jestli je soused "stejný terén" jako ona.
    // Diagonální bit se počítá jen když jsou obě sousední kardinální strany taky stejné (viz BlobTileset).
    public int ComputeBitmask(int x, int y)
    {
        TerrainType center = Terrain[x, y];

        bool n = IsSameTerrain(x, y - 1, center);
        bool e = IsSameTerrain(x + 1, y, center);
        bool s = IsSameTerrain(x, y + 1, center);
        bool w = IsSameTerrain(x - 1, y, center);
        bool ne = IsSameTerrain(x + 1, y - 1, center);
        bool se = IsSameTerrain(x + 1, y + 1, center);
        bool sw = IsSameTerrain(x - 1, y + 1, center);
        bool nw = IsSameTerrain(x - 1, y - 1, center);

        int bitmask = 0;
        if (n) bitmask |= BitN;
        if (e) bitmask |= BitE;
        if (s) bitmask |= BitS;
        if (w) bitmask |= BitW;
        if (ne && n && e) bitmask |= BitNE;
        if (se && s && e) bitmask |= BitSE;
        if (sw && s && w) bitmask |= BitSW;
        if (nw && n && w) bitmask |= BitNW;

        return bitmask;
    }

    private bool IsSameTerrain(int x, int y, TerrainType type)
    {
        if (x < 0 || y < 0 || x >= Terrain.GetLength(0) || y >= Terrain.GetLength(1))
            return false; // mimo mapu se nikdy nepočítá jako "stejné"

        if (IsWater(x, y))
            return false; // voda vždy překryje terén pod sebou

        return Terrain[x, y] == type;
    }

    public static Point PixelToGrid(Vector2 pixelPos)
    {
        return new Point((int)Math.Floor(pixelPos.X / TileSize), (int)Math.Floor(pixelPos.Y / TileSize));
    }

    public static Vector2 GridToPixel(Point gridPos)
    {
        return new Vector2(gridPos.X * TileSize, gridPos.Y * TileSize);
    }

    public bool TryGetResourceAt(Vector2 pixelPosition, out ResourceTile tile)
    {
        return Resources.TryGetValue(PixelToGrid(pixelPosition), out tile);
    }

    public bool IsWater(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Terrain.GetLength(0) || y >= Terrain.GetLength(1))
            return true; // mimo mapu = moře
        return Water[x, y];
    }
    public void RemoveIfDepleted(Point gridPos)
    {
        if (Resources.TryGetValue(gridPos, out ResourceTile tile) && tile.Remaining <= 0)
            Resources.Remove(gridPos);
    }
}