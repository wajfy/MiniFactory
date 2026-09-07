using Microsoft.Xna.Framework.Graphics;

public class Inventory
{
    public InventorySlot[,] Slots { get; set; }
    public static int StackSize = 100;

    public Inventory(int columns, int rows)
    {
        Slots = new InventorySlot[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Slots[row, col] = new InventorySlot(ItemType.EmptySlot, 0);
            }
        }
    }
    public bool TryAddItem(ItemType type, int count = 1)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Type == type && slot.Count < StackSize)
            {
                slot.Count += count;
                return true;
            }
        }
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Count == 0)
            {
                slot.Type = type;
                slot.Count = count;
                return true;
            }
        }

        return false;
    }
    public void MoveItem(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (fromRow == toRow && fromCol == toCol) return; // pustil na stejný slot, nic se neděje
        InventorySlot from = Slots[fromRow, fromCol];
        InventorySlot to = Slots[toRow, toCol];

        if (from.Type == to.Type)
        {
            to.Count += from.Count;

            if (to.Count > StackSize)
            {
                int rest = to.Count - StackSize;
                from.Count = rest;
                to.Count = StackSize;
            }
            else
            {
                from.Count = 0;
            }
            return;
        }

        ItemType tempType = from.Type;
        int tempCount = from.Count;

        from.Type = to.Type;
        from.Count = to.Count;

        to.Type = tempType;
        to.Count = tempCount;
    }
}