using System;

namespace CardinalInventoryApp.Contracts
{
    public class StockItem
    {
        public Guid StockItemId { get; set; }
        //public Guid StockItemCategoryId { get; set; }
        //public StockItemCategory StockItemCategory { get; set; }
        public string Description { get; set; }
        public int UnitSizeMilliliters { get; set; }
        public Decimal UnitCost { get; set; }
        public string ImagePath { get; set; }
        public string SKU { get; set; }
    }
}
