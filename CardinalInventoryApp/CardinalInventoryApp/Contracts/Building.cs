using System;

namespace CardinalInventoryApp.Contracts
{
    public class Building
    {
        public Guid BuildingId { get; set; }
        public string Description { get; set; }
        public Guid BarId { get; set; }
        public object Bar { get; set; }
        public bool Active { get; set; }
    }
}