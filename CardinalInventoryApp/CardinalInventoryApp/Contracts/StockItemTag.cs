using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryApp.Contracts
{
    public class StockItemTag
    {
        public Guid StockItemTagId { get; set; }
        public Guid StockItemId { get; set; }
        public object StockItem { get; set; }
        public string Tag { get; set; }
    }
}
