public class Inventory
{
    public InventorySlot[,] Slots { get; set; }

    public Inventory(int columns, int rows)
    {
        Slots = new InventorySlot[columns, rows];
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                Slots[i, j] = new InventorySlot(ItemType.EmptySlot, 0);
            }
        }
        Slots[0, 0] = new InventorySlot(ItemType.Stone, 5);
    }
}