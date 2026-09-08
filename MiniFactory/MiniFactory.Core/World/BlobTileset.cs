using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BlobTileset
{
    private const int TileSourceSize = 16;

    // bitmask -> (col, row) v pixel-artové mřížce zdrojové textury.
    // Konvence bitů: N=1, NE=2, E=4, SE=8, S=16, SW=32, W=64, NW=128.
    private static readonly Dictionary<int, Point> BitmaskToCell = new Dictionary<int, Point>
    {
        { 0, new Point(3, 3) }, { 1, new Point(3, 2) }, { 4, new Point(0, 3) }, { 5, new Point(4, 3) },
        { 7, new Point(0, 2) }, { 16, new Point(3, 0) }, { 17, new Point(3, 1) }, { 20, new Point(4, 0) },
        { 21, new Point(4, 4) }, { 23, new Point(4, 1) }, { 28, new Point(0, 0) }, { 29, new Point(4, 2) },
        { 31, new Point(0, 1) }, { 64, new Point(2, 3) }, { 65, new Point(7, 3) }, { 68, new Point(1, 3) },
        { 69, new Point(8, 3) }, { 71, new Point(6, 3) }, { 80, new Point(7, 0) }, { 81, new Point(7, 4) },
        { 84, new Point(8, 0) }, { 85, new Point(8, 4) }, { 87, new Point(9, 3) }, { 92, new Point(6, 0) },
        { 93, new Point(9, 2) }, { 95, new Point(6, 4) }, { 112, new Point(2, 0) }, { 113, new Point(7, 2) },
        { 116, new Point(5, 0) }, { 117, new Point(10, 2) }, { 119, new Point(9, 1) }, { 124, new Point(1, 0) },
        { 125, new Point(8, 2) }, { 127, new Point(6, 2) }, { 193, new Point(2, 2) }, { 197, new Point(5, 3) },
        { 199, new Point(1, 2) }, { 209, new Point(7, 1) }, { 213, new Point(10, 3) }, { 215, new Point(8, 1) },
        { 221, new Point(9, 0) }, { 223, new Point(6, 1) }, { 241, new Point(2, 1) }, { 245, new Point(5, 4) },
        { 247, new Point(5, 1) }, { 253, new Point(5, 2) }, { 255, new Point(1, 1) },
    };

    public Texture2D Texture { get; }

    public BlobTileset(Texture2D texture)
    {
        Texture = texture;
    }

    public Rectangle GetSourceRect(int bitmask)
    {
        if (!BitmaskToCell.TryGetValue(bitmask, out Point cell))
            cell = BitmaskToCell[255]; // neplatná kombinace -> radši plná dlaždice než pád

        return new Rectangle(cell.X * TileSourceSize, cell.Y * TileSourceSize, TileSourceSize, TileSourceSize);
    }
}
