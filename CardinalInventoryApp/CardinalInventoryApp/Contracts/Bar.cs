using System;

namespace CardinalInventoryApp.Contracts
{
    public class Bar
    {
        public Guid BarId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
