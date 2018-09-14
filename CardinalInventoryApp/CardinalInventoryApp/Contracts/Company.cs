using System;

namespace CardinalInventoryApp.Contracts
{
    public class Company
    {
        public Guid CompanyId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
