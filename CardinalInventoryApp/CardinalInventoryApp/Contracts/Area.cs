using System;

namespace CardinalInventoryApp.Contracts
{
    public class Area
    {
        public Guid AreaId { get; set; }
        public string Description { get; set; }
        public Guid BuildingId { get; set; }
        public object Building { get; set; }
        public bool Active { get; set; }
    }
}
