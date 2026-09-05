public class InventorySlot
{
    public ItemType Type { get; set; }
    public int Count { get; set; }
    public InventorySlot(ItemType type, int count)
    {
        Type = type;
        Count = count;
    }
}