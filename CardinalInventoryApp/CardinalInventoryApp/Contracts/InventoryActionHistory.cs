using System;

namespace CardinalInventoryApp.Contracts
{
    public class InventoryActionHistory
    {
        public Guid InventoryActionHistoryId { get; set; }
        public Guid AreaId { get; set; }
        public object Area { get; set; }
        public Guid StockItemId { get; set; }
        public object StockItem { get; set; }
        public Guid ApplicationUserId { get; set; }
        public object ApplicationUser { get; set; }
        public DateTime Timestamp { get; set; }
        public Decimal ItemLevel { get; set; }
        public InventoryAction Action { get; set; }
    }
}
