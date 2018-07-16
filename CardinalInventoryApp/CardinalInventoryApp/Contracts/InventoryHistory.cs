using System;

namespace CardinalInventoryApp.Contracts
{
    public class InventoryHistory
    {
        public Guid InventoryHistoryId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid AreaId { get; set; }
        public object Area { get; set; }
        public Guid StockItemId { get; set; }
        public object StockItem { get; set; }
        public Decimal Quantity { get; set; }
    }
}
