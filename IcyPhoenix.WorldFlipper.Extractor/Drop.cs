namespace IcyPhoenix.WorldFlipper.Extractor
{
    public class Drop
    {
        public string DropName { get; set; }
        public bool IsRareDrop { get; set; }
        public string ItemDropID { get; set; }
        public int Quantity { get; set; }
        public double Chance { get; set; }
        public bool IsRareChestIcon { get; set; }
        public DropType DropType { get; set; }

    }

    public enum DropType
    {
        Item = 0,
        Armant = 1,
        Character = 2,
        Stone = 3,
        Mana = 4,
        PooledExp = 5,
        Element = 6,
    }
}